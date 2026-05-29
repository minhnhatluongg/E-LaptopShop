using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IBannerRepository : IBaseRepository<Banner>
    {
        /// <summary>Lấy banner đang active theo vị trí, sort theo DisplayOrder.</summary>
        Task<IEnumerable<Banner>> GetActiveByPositionAsync(
            string position, CancellationToken ct = default);
    }
}
