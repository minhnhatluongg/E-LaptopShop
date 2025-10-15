using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Implementations
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserAuthRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwt;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        private readonly IDateTimeProvider _time;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleLookup _roleLookup;

        private readonly Lazy<Task<int>> _customerRoleId;
        private readonly Lazy<Task<string>> _customerRoleName;

        public AuthService(
        IUserAuthRepository users,
        IPasswordHasher hasher,
        IJwtService jwt,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger,
        IDateTimeProvider time,
        IMapper mapper,
        IRoleLookup roleLookup,
        IUserRepository userRepository)
        {
            _users = users;
            _hasher = hasher;
            _jwt = jwt;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
            _time = time;
            _userRepository = userRepository;
            _mapper = mapper;
            _roleLookup = roleLookup;

            _customerRoleId = new Lazy<Task<int>>(() =>
            _roleLookup.GetIdByCodeAsync(RoleCodes.Customer, CancellationToken.None));

            _customerRoleName = new Lazy<Task<string>>(() =>
                _roleLookup.GetNameByCodeAsync(RoleCodes.Customer, CancellationToken.None));
        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
        {
            Throw.IfNullOrEmpty(dto.Email, nameof(dto.Email));
            Throw.IfNullOrEmpty(dto.Password, nameof(dto.Password));

            var user = await _users.GetByEmailForAuthAsync(dto.Email, ct);
            if (user == null)
            {
                Throw.Unauthorized("Invalid email or password");
            }
            if (await _users.IsUserLockedAsync(user.Id, ct)) { Throw.Forbidden("User", "login"); }
            if (!user.IsActive) Throw.Forbidden("User", "login");

            var passOk = !string.IsNullOrEmpty(user.PasswordHash) && _hasher.VerifyHashedPassword(dto.Password!, user.PasswordHash);
            if (!passOk)
            {
                await _users.IncrementLoginAttemptsAsync(user.Id, ct);
                var nextAttemps = user.LoginAttempts + 1;
                if (nextAttemps >= _jwtSettings.MaxFailedAccessAttempts)
                {
                    await _users.LockUserAsync(user.Id, DateTime.UtcNow.AddMinutes(_jwtSettings.LockoutDurationMinutes), ct);

                }
                Throw.Unauthorized("Invalid email or password");
            }
            await _users.ResetLoginAttemptsAsync(user.Id, ct);
            await _users.UpdateLastLoginAsync(user.Id, ct);

            var tokens = await _jwt.GenerateTokensAsync(user, ct);
                
            return new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiration = tokens.AccessTokenExpiration,
                RefreshTokenExpiration = tokens.RefreshTokenExpiration,
                ExpiresIn = tokens.ExpiresIn,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = await _customerRoleName.Value,
                    EmailConfirmed = user.EmailConfirmed,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                Throw.IfNullOrEmpty(refreshToken, nameof(refreshToken));
            JwtSecurityToken jwt;
            try
            {
                jwt = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot read refresh token");
                throw new SecurityTokenException("Invalid refresh token format");
            }
            var typ = jwt.Claims.FirstOrDefault(c => c.Type == "typ")?.Value;
            if (!string.Equals(typ, "refresh", StringComparison.OrdinalIgnoreCase))
                throw new SecurityTokenException("Invalid token type");

            var userIdStr = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdStr, out var userId))
                throw new SecurityTokenException("Invalid user id in token");

            var recognized = await _users.IsValidRefreshTokenAsync(userId, refreshToken, ct);
            if (!recognized)
                throw new SecurityTokenException("Refresh token is revoked or not recognized");

            await _users.RevokeRefreshTokenAsync(userId, ct);
            _logger.LogInformation("User {UserId} logged out (refresh token revoked).", userId);
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken ct = default)
        {
            Throw.IfNullOrEmpty(dto.RefreshToken, nameof(dto.RefreshToken));
            Throw.IfNullOrEmpty(dto.AccessToken, nameof(dto.AccessToken));

            var principal = _jwt.GetPrincipalFromExpiredToken(dto.AccessToken);
            var userId = int.Parse(principal.FindFirst("sub")!.Value);

            var valid = await _users.IsValidRefreshTokenAsync(userId, dto.RefreshToken, ct);
            if (!valid) Throw.Unauthorized("Invalid or expired refresh token");

            var user = await _users.GetByIdWithRoleAsync(userId, ct);
            Throw.IfNull(user, nameof(User), userId);

            var tokens = await _jwt.GenerateTokensAsync(user!, ct);

            await _users.UpdateRefreshTokenAsync(user!.Id, tokens.RefreshToken, tokens.RefreshTokenExpiration, ct);

            return new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpiration = tokens.AccessTokenExpiration,
                RefreshTokenExpiration = tokens.RefreshTokenExpiration,
                ExpiresIn = tokens.ExpiresIn,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role?.Name ?? "Customer",
                    EmailConfirmed = user.EmailConfirmed,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
        {
            Throw.IfNull(dto.Email, nameof(dto.Email));
            Throw.IfNull(dto.Password, nameof(dto.Password));

            var email = dto.Email.Trim().ToLowerInvariant();
            var checkMail = !await _userRepository.IsEmailUniqueAsync(email, null, ct);
            if (checkMail)
            {
                Throw.Conflict("Email already in use", new object[] { email });
            }

            var passwordHash = _hasher.HashPassword(dto.Password);
            var customerRoleId = await _customerRoleId.Value;

            var now = _time.UtcNow;
            var user = _mapper.Map<User>(dto);

            user.Email = email;                      
            user.PasswordHash = _hasher.HashPassword(dto.Password);
            user.RoleId = customerRoleId;
            user.IsActive = true;
            user.EmailConfirmed = false;
            user.CreatedAt = _time.UtcNow; 

            var  created = await _userRepository.AddAsync(user, ct);

            return _mapper.Map<UserDto>(created);
        }
    }
}
