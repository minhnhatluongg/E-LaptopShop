using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IUserWalletRepository : IBaseRepository<UserWallet>
    {
        Task<UserWallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy ví của user; nếu chưa có thì tạo mới (idempotent).
        /// </summary>
        Task<UserWallet> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default);
    }
}
