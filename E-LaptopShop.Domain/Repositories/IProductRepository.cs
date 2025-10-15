using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetFilteredAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    // Advanced query operations for service layer
    IQueryable<Product> GetProductsQueryable(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null);

    // Command operations
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

} 