using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IShoppingCartItemRepository : IBaseRepository<ShoppingCartItem>
    {
        Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem> GetByCartAndProductAsync(int cartId, int productId, CancellationToken cancellationToken = default);
        Task<int> DeleteByCartIdAsync(int cartId, CancellationToken cancellationToken = default);
    }
}
