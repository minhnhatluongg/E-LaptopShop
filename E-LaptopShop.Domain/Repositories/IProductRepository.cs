using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> GetFilteredAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    IQueryable<Product> GetProductsQueryable(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null);
}