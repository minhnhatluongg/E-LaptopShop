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
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtService(
            IOptions<JwtSettings> jwtSettings,
            IUserAuthRepository userAuthRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _userAuthRepository = userAuthRepository;
            _tokenHandler = new JwtSecurityTokenHandler();
            
            // Cấu hình validation parameters
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                RequireExpirationTime = true
            };
        }

        public async Task<TokenResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
        {
            // Generate Access Token
            var accessToken = GenerateAccessToken(user);
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            
            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            
            // Save refresh token to database
            await _userAuthRepository.UpdateRefreshTokenAsync(
                user.Id, 
                refreshToken, 
                refreshTokenExpiration, 
                cancellationToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                ExpiresIn = (int)TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes).TotalSeconds,
                Roles = user.Role != null ? new[] { user.Role.Name } : Array.Empty<string>()
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var principal = GetPrincipalFromExpiredToken(refreshToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid refresh token");

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new SecurityTokenException("Invalid user ID in token");

            // Validate refresh token in database
            var isValidRefreshToken = await _userAuthRepository.IsValidRefreshTokenAsync(
                userId, refreshToken, cancellationToken);
            
            if (!isValidRefreshToken)
                throw new SecurityTokenException("Invalid or expired refresh token");

            // Get fresh user data
            var user = await _userAuthRepository.GetByIdWithRoleAsync(userId, cancellationToken);
            
            if (user == null || !user.IsActive)
                throw new SecurityTokenException("User not found or inactive");

            // Revoke old refresh token
            await _userAuthRepository.RevokeRefreshTokenAsync(userId, cancellationToken);

            // Generate new tokens
            return await GenerateTokensAsync(user, cancellationToken);
        }

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FullName),
                new("user_id", user.Id.ToString()),
                new("email", user.Email),
                new("full_name", user.FullName),
                new("email_confirmed", user.EmailConfirmed.ToString().ToLower()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // 2025: Token ID
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add role claims
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
                claims.Add(new Claim("role", user.Role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false; // 2025: Allow expired tokens for refresh

            try
            {
                var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                
                if (validatedToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

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
                var jwt = _tokenHandler.ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<string> GetRolesFromToken(string token)
        {
            try
            {
                var jwt = _tokenHandler.ReadJwtToken(token);
                return jwt.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}