using E_LaptopShop.Domain.Entities;
using System.Security.Claims;

namespace E_LaptopShop.Application.Services
{
    public interface IJwtService
    {
        Task<TokenResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> ValidateTokenAsync(string token);
        Task RevokeTokenAsync(int userId, CancellationToken cancellationToken = default);
        Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        string? GetUserIdFromToken(string token);
        IEnumerable<string> GetRolesFromToken(string token);
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } // seconds
        
        // 2025 Enhancement: Token metadata
        public string Scope { get; set; } = "api";
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}