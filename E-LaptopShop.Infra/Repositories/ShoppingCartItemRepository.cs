using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class ShoppingCartItemRepository : IShoppingCartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ShoppingCartItem> AddAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                item.AddedAt = DateTime.UtcNow;

                await _context.ShoppingCartItems.AddAsync(item, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return item;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding shopping cart item", ex);
            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _context.ShoppingCartItems.FindAsync(new object[] { id }, cancellationToken);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                return await _context.SaveChangesAsync(cancellationToken);
            }
            return 0;
        }

        public async Task<int> DeleteByCartIdAsync(int cartId, CancellationToken cancellationToken = default)
        {
            var items = await _context.ShoppingCartItems
                                .Where(i => i.ShoppingCartId == cartId)
                                .ToListAsync(cancellationToken);
            _context.ShoppingCartItems.RemoveRange(items);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ShoppingCartItems.AnyAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<ShoppingCartItem> GetByCartAndProductAsync(int cartId, int productId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Trả về null nếu không tìm thấy (đây là hành vi bình thường)
                var items = await _context.ShoppingCartItems
                            .FirstOrDefaultAsync(i => i.ShoppingCartId == cartId && i.ProductId == productId, cancellationToken);
                return items;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving shopping cart item for cart ID {cartId} and product ID {productId}", ex);
            }
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId, CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _context.ShoppingCartItems
                    .Include(i => i.Product)
                    .Include(i => i.ShoppingCart)
                    .AsNoTracking()
                    .Where(i => i.ShoppingCartId == cartId)
                    .ToListAsync(cancellationToken);
                if (items == null || !items.Any())
                    throw new KeyNotFoundException($"No shopping cart items found for cart ID {cartId}");
                return items;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving shopping cart items for cart ID {cartId}", ex);
            }
        }

        public async Task<ShoppingCartItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _context.ShoppingCartItems
                    .Include(i => i.Product)
                    .Include(i => i.ShoppingCart)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
                if(item == null)
                    throw new KeyNotFoundException($"Shopping cart item with ID {id} not found");
                return item;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving shopping cart item with ID {id}", ex);
            }
        }

        public async Task<ShoppingCartItem> UpdateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.ShoppingCartItems.Update(item);
                await _context.SaveChangesAsync(cancellationToken);
                return item;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating shopping cart item with ID {item.Id}", ex);
            }
        }
    }
}
