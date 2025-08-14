using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Order> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order> UpdateStatusAsync(int orderId, OrderStatus status, string updatedBy, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> CanCancelAsync(int orderId, CancellationToken cancellationToken = default);
        Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetTotalCountByUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
