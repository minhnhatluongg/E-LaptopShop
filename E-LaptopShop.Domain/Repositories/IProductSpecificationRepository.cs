using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories;

public interface IProductSpecificationRepository
{
    Task<ProductSpecification> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ProductSpecification>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ProductSpecification>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
    Task<ProductSpecification> AddAsync(ProductSpecification spec, CancellationToken cancellationToken);
    Task<ProductSpecification> UpdateAsync(ProductSpecification spec, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
} 