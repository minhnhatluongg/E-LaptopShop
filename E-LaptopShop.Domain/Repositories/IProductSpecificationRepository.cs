using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;

namespace E_LaptopShop.Domain.Repositories;

public interface IProductSpecificationRepository : IBaseRepository<ProductSpecification>
{
    Task<IEnumerable<ProductSpecification>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
} 