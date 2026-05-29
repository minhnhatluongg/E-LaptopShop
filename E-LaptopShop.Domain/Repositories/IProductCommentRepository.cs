using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IProductCommentRepository
    {
        Task<IEnumerable<ProductComment>> GetByProductIdAsync(int productId, CancellationToken ct = default);
        Task<ProductComment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductComment> AddAsync(ProductComment comment, CancellationToken ct = default);
        Task<int> SoftDeleteAsync(int id, CancellationToken ct = default);
        Task<int> CountByProductIdAsync(int productId, CancellationToken ct = default);
    }
}
