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
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            try
            {
                shoppingCart.CreatedAt = DateTime.UtcNow;
                shoppingCart.UpdatedAt = DateTime.UtcNow;

                await _context.ShoppingCarts.AddAsync(shoppingCart, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return shoppingCart;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding shopping cart", ex);
            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await _context.ShoppingCarts.FindAsync(new object[] { id }, cancellationToken);
                if (cart != null)
                {
                    _context.ShoppingCarts.Remove(cart);
                    return await _context.SaveChangesAsync(cancellationToken);
                }
                return 0;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Error deleting shopping cart with ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while deleting the shopping cart", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ShoppingCarts.AnyAsync(sc => sc.Id == id, cancellationToken);
        }

        public async Task<ShoppingCart> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                             .Include(sc => sc.Items)
                             .ThenInclude(i => i.Product)
                             .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
                if (cart == null)
                {
                    throw new KeyNotFoundException($"Shopping cart with ID {id} not found");
                }
                return cart;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving shopping cart with ID {id}", ex);
            }
        }

        public async Task<ShoppingCart> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                    .Include(sc => sc.Items)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(sc => sc.UserId == userId, cancellationToken);

                if (cart == null)
                {
                    throw new KeyNotFoundException($"No shopping cart found for user ID {userId}");
                }

                return cart;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error retrieving shopping cart for user ID {userId}", ex);
            }
        }

        public async Task<ShoppingCart> GetCartWithItemsAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await _context.ShoppingCarts
                                    .Include(sc => sc.Items)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.ProductImages)
                                    .ThenInclude(pi => pi.SysFile)
                                    .FirstOrDefaultAsync(sc => sc.UserId == userId, cancellationToken);

                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        TotalAmount = 0
                    };
                    await _context.ShoppingCarts.AddAsync(cart, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return cart;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving shopping cart with items", ex);
            }
        }

        public async Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            try
            {
                shoppingCart.UpdatedAt = DateTime.UtcNow;

                _context.ShoppingCarts.Update(shoppingCart);
                await _context.SaveChangesAsync(cancellationToken);
                return shoppingCart;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating shopping cart", ex);
            }
        }
    }
}
