using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Infra.Repositories
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailForAuthAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdWithRoleAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = expiry;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RevokeRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task IncrementLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.LoginAttempts++;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task ResetLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.LoginAttempts = 0;
                user.IsLocked = false;
                user.LockedUntil = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> IsValidRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null || 
                string.IsNullOrEmpty(user.RefreshToken) ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task LockUserAsync(int userId, DateTime lockUntil, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.IsLocked = true;
                user.LockedUntil = lockUntil;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UnlockUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.IsLocked = false;
                user.LockedUntil = null;
                user.LoginAttempts = 0;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> IsUserLockedAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null) return false;

            // Check if user is locked and lock period has expired
            if (user.IsLocked && user.LockedUntil.HasValue && user.LockedUntil.Value <= DateTime.UtcNow)
            {
                // Auto-unlock user if lock period has expired
                await UnlockUserAsync(userId, cancellationToken);
                return false;
            }

            return user.IsLocked;
        }
    }
}
