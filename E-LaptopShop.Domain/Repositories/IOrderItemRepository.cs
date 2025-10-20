using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<OrderItem> CreateAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
        Task<OrderItem> UpdateStatusAsync(int itemId, OrderItemStatus status, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    }
}
