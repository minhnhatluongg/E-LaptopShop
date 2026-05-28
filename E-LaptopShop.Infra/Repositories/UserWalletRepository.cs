using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class UserWalletRepository : BaseRepository<UserWallet>, IUserWalletRepository
    {
        public UserWalletRepository(ApplicationDbContext context, ILogger<UserWalletRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<UserWallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
        }

        public async Task<UserWallet> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Set<UserWallet>()
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
            if (existing != null) return existing;

            var wallet = new UserWallet
            {
                UserId = userId,
                Balance = 0m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
            await _context.Set<UserWallet>().AddAsync(wallet, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return wallet;
        }
    }
}
