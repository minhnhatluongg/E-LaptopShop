using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IShoppingCartItemRepository
    {
        Task<ShoppingCartItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem> GetByCartAndProductAsync(int cartId, int productId, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem> AddAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem> UpdateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<int> DeleteByCartIdAsync(int cartId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
