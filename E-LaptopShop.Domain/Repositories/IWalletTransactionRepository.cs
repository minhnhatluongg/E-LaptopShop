using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IWalletTransactionRepository : IBaseRepository<WalletTransaction, long>
    {
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(
            int walletId,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<WalletTransaction>> GetByUserIdAsync(
            int userId,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
}
