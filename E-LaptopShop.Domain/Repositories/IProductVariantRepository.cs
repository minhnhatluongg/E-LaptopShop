using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IProductVariantRepository : IBaseRepository<ProductVariant>
    {
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(
            int productId, CancellationToken ct = default);

        Task<ProductVariant?> GetBySKUAsync(
            string sku, CancellationToken ct = default);

        Task<bool> SKUExistsAsync(
            string sku, int? excludeId = null, CancellationToken ct = default);

        Task<ProductVariant> AddWithAttributesAsync(
            ProductVariant variant,
            IEnumerable<int> attributeValueIds,
            CancellationToken ct = default);

        Task DeleteByProductIdAsync(int productId, CancellationToken ct = default);
    }
}
