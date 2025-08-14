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
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                            .ThenInclude(pi => pi.SysFile)
                    .FirstOrDefaultAsync(oi => oi.Id == id, cancellationToken);

                if (orderItem == null)
                {
                    throw new KeyNotFoundException($"Order item with ID {id} not found");
                }

                return orderItem;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving order item with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                            .ThenInclude(pi => pi.SysFile)
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync(cancellationToken);

                return orderItems;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving order items for order {orderId}", ex);
            }
        }

        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Product)
                    .Where(oi => oi.ProductId == productId)
                    .OrderByDescending(oi => oi.Order.OrderDate)
                    .ToListAsync(cancellationToken);

                return orderItems;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving order items for product {productId}", ex);
            }
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.OrderItems.AddAsync(orderItem, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return orderItem;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error creating order item", ex);
            }
        }

        public async Task<OrderItem> UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.OrderItems.Update(orderItem);
                await _context.SaveChangesAsync(cancellationToken);

                return orderItem;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating order item with ID {orderItem.Id}", ex);
            }
        }

        public async Task<OrderItem> UpdateStatusAsync(int itemId, OrderItemStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(new object[] { itemId }, cancellationToken);
                if (orderItem == null)
                {
                    throw new KeyNotFoundException($"Order item with ID {itemId} not found");
                }

                orderItem.Status = status.ToString();
                await _context.SaveChangesAsync(cancellationToken);

                return orderItem;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating order item status for ID {itemId}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(new object[] { id }, cancellationToken);
                if (orderItem == null)
                {
                    return false;
                }

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting order item with ID {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.OrderItems.AnyAsync(oi => oi.Id == id, cancellationToken);
        }

        public async Task<decimal> GetTotalByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var total = await _context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .SumAsync(oi => oi.SubTotal, cancellationToken);

                return total;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error calculating total for order {orderId}", ex);
            }
        }
    }
}
