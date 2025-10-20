using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IShoppingCartRepository : IBaseRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<ShoppingCart> GetCartWithItemsAsync(int userId, CancellationToken cancellationToken = default);
    }
}
