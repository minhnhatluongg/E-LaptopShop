using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IProductAttributeRepository : IBaseRepository<ProductAttribute>
    {
        Task<IEnumerable<ProductAttribute>> GetAllActiveWithValuesAsync(
            CancellationToken ct = default);

        Task<ProductAttributeValue?> GetValueByIdAsync(
            int valueId, CancellationToken ct = default);

        Task<ProductAttributeValue> AddValueAsync(
            ProductAttributeValue value, CancellationToken ct = default);

        Task<bool> DeleteValueAsync(int valueId, CancellationToken ct = default);
    }
}
