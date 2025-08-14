using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.ProductImages)
                                .ThenInclude(pi => pi.SysFile)
                    .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found");
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving order with ID {id}", ex);
            }
        }

        public async Task<Order> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with number {orderNumber} not found");
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving order with number {orderNumber}", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return orders;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving orders for user {userId}", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return orders;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving all orders", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var statusString = status.ToString();
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .Where(o => o.Status == statusString)
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return orders;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving orders with status {status}", ex);
            }
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            try
            {
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.Orders.AddAsync(order, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return order;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error creating order", ex);
            }
        }

        public async Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            try
            {
                order.UpdatedAt = DateTime.UtcNow;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                return order;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating order with ID {order.Id}", ex);
            }
        }

        public async Task<Order> UpdateStatusAsync(int orderId, OrderStatus status, string updatedBy, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found");
                }

                order.Status = status.ToString();
                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = updatedBy;

                // Update specific fields based on status
                switch (status)
                {
                    case OrderStatus.Delivered:
                        // Mark as paid if not already
                        if (!order.IsPaid)
                        {
                            order.IsPaid = true;
                            order.PaidDate = DateTime.UtcNow;
                        }
                        break;
                }

                await _context.SaveChangesAsync(cancellationToken);
                return order;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating order status for ID {orderId}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<bool> CanCancelAsync(int orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
                if (order == null)
                {
                    return false;
                }

                var status = Enum.Parse<OrderStatus>(order.Status);
                
                // Can only cancel orders in Pending or Confirmed status
                return status == OrderStatus.Pending || status == OrderStatus.Confirmed;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders.CountAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .CountAsync(cancellationToken);
        }
    }
}
