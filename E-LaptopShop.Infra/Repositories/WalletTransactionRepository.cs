using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction, long>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(ApplicationDbContext context, ILogger<WalletTransactionRepository> logger)
            : base(context, logger)
        {
        }

        protected override long GetEntityId(WalletTransaction entity) => entity.Id;

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(
            int walletId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WalletTransaction>> GetByUserIdAsync(
            int userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
