using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface ICouponRepository : IBaseRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>Atomically increment UsedCount. Returns new value.</summary>
        Task<int> IncrementUsedCountAsync(int couponId, CancellationToken cancellationToken = default);

        Task<int> CountUserUsageAsync(int couponId, int userId, CancellationToken cancellationToken = default);

        Task<CouponUsage> AddUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default);
    }
}
