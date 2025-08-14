using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ShoppingCart> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<ShoppingCart> AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
        Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<ShoppingCart> GetCartWithItemsAsync(int userId, CancellationToken cancellationToken = default);
    }
}
