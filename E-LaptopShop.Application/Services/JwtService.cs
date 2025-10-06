using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_LaptopShop.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();


        private readonly TokenValidationParameters _accessValidateParams;
        private readonly TokenValidationParameters _refreshValidateParams;
        private readonly SigningCredentials _signingCreds;


        public JwtService(
            IOptions<JwtSettings> jwtSettings,
            IUserAuthRepository userAuthRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _userAuthRepository = userAuthRepository;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            _signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            _accessValidateParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = key,
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                RequireExpirationTime = true
            };

            _refreshValidateParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true, 
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                RequireExpirationTime = true
            };
        }

        public async Task<TokenResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var now = DateTime.UtcNow;

            // ===== ACCESS (typ=access) =====
            var accessClaims = BuildAccessClaims(user);
            var accessExpires = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var accessJwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: accessClaims,
                notBefore: now,
                expires: accessExpires,
                signingCredentials: _signingCreds);
            var accessToken = _tokenHandler.WriteToken(accessJwt);

            // ===== REFRESH (typ=refresh) =====
            var refreshClaims = BuildRefreshClaims(user);
            var refreshExpires = now.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            var refreshJwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: refreshClaims,
                notBefore: now,
                expires: refreshExpires,
                signingCredentials: _signingCreds);
            var refreshToken = _tokenHandler.WriteToken(refreshJwt);

            // Lưu/rotate refresh token trong DB để có thể revoke
            await _userAuthRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshExpires, cancellationToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessExpires,
                RefreshTokenExpiration = refreshExpires,
                ExpiresIn = (int)(accessExpires - now).TotalSeconds,
                Roles = user.Role != null ? new[] { user.Role.Name } : Array.Empty<string>()
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new SecurityTokenException("Refresh token is required");

            // Validate refresh JWT (còn hạn + ký đúng + đúng alg)
            var principal = ValidateToken(refreshToken, _refreshValidateParams);
            if (principal == null) throw new SecurityTokenException("Invalid or expired refresh token");

            // Bảo vệ: chỉ chấp nhận typ=refresh
            var jwt = _tokenHandler.ReadJwtToken(refreshToken);
            var typ = jwt.Claims.FirstOrDefault(c => c.Type == "typ")?.Value;
            if (!string.Equals(typ, "refresh", StringComparison.OrdinalIgnoreCase))
                throw new SecurityTokenException("Invalid token type");

            var userIdClaim = principal.FindFirst("sub")?.Value
                           ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                throw new SecurityTokenException("Invalid user id in token");

            // Đối chiếu refresh token hiện tại trong DB
            var isValid = await _userAuthRepository.IsValidRefreshTokenAsync(userId, refreshToken, cancellationToken);
            if (!isValid) throw new SecurityTokenException("Refresh token is revoked or not recognized");

            var user = await _userAuthRepository.GetByIdWithRoleAsync(userId, cancellationToken)
                       ?? throw new SecurityTokenException("User not found");
            if (!user.IsActive) throw new SecurityTokenException("User inactive");

            // Rotate: phát hành cặp mới và update DB
            return await GenerateTokensAsync(user, cancellationToken);
        }
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _tokenHandler.ValidateToken(token, _accessValidateParams, out var validated);
                return validated is JwtSecurityToken jwt &&
                       jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            // DÙNG CHO ACCESS TOKEN đã hết hạn (đọc claim mà không cần hạn)
            var p = _accessValidateParams.Clone();
            p.ValidateLifetime = false;

            try
            {
                var principal = _tokenHandler.ValidateToken(token, p, out var validated);
                if (validated is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                // Nếu có claim typ thì chỉ chấp nhận access
                var typ = jwt.Claims.FirstOrDefault(c => c.Type == "typ")?.Value;
                if (!string.IsNullOrEmpty(typ) && !typ.Equals("access", StringComparison.OrdinalIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task RevokeTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            await _userAuthRepository.RevokeRefreshTokenAsync(userId, cancellationToken);
        }

        public async Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            await _userAuthRepository.RevokeRefreshTokenAsync(userId, cancellationToken);
        }

        public string? GetUserIdFromToken(string token)
        {
            try
            {
                ClaimsPrincipal? principal;
                try
                {
                    principal = _tokenHandler.ValidateToken(token, _accessValidateParams, out _);
                }
                catch (SecurityTokenExpiredException)
                {
                    principal = GetPrincipalFromExpiredToken(token);
                }

                var id = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return id;
            }
            catch { return null; }
        }
        private ClaimsPrincipal? ValidateToken(string token, TokenValidationParameters p)
        {
            try
            {
                var principal = _tokenHandler.ValidateToken(token, p, out var validated);
                if (validated is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;
                return principal;
            }
            catch { return null; }
        }

        public IEnumerable<string> GetRolesFromToken(string token)
        {
            try
            {
                var jwt = _tokenHandler.ReadJwtToken(token);
                return jwt.Claims
                          .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                          .Select(c => c.Value)
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .ToArray();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private static IEnumerable<Claim> BuildAccessClaims(User user)
        {
            var claims = new List<Claim>
            {
                new("sub", user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new("typ", "access"),
                new(ClaimTypes.Name, user.FullName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (!string.IsNullOrWhiteSpace(user.Role?.Name))
            {
                claims.Add(new(ClaimTypes.Role, user.Role!.Name));
                claims.Add(new("role", user.Role!.Name)); // optional
            }

            return claims;
        }

        private static IEnumerable<Claim> BuildRefreshClaims(User user)
        {
            return new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim("typ", "refresh")
            };
        }
    }
}