using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetFilteredAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
} 