using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IUserAuthRepository
    {
        Task<User?> GetByEmailForAuthAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithRoleAsync(int userId, CancellationToken cancellationToken = default);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(int userId, CancellationToken cancellationToken = default);
        Task IncrementLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
        Task ResetLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
        Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> IsValidRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default);
        Task LockUserAsync(int userId, DateTime lockUntil, CancellationToken cancellationToken = default);
        Task UnlockUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> IsUserLockedAsync(int userId, CancellationToken cancellationToken = default);
    }
}