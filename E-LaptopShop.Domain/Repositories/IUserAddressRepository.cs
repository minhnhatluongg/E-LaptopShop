using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories 
{
    public interface IUserAddressRepository : IBaseRepository<UserAddress>
    {
        IQueryable<UserAddress> GetFilteredQueryable(UserAddressFilterParams filter, bool includeUser = false);
        IQueryable<UserAddress> Query(); 
        IQueryable<UserAddress> QueryIgnoreFilters(); 

        Task<UserAddress?> FindAsync(int id, int userId, CancellationToken ct);
        Task<UserAddress?> FindByIdAsync(int id, CancellationToken ct);
        Task<int> UnsetDefaultForUserAsync(int userId, int skipId, CancellationToken ct);
        EntityEntry<UserAddress> Entry(UserAddress entity); 
        Task<int> HardDeleteAsync(int id, CancellationToken ct);

        // Hỗ trợ rule “1 mặc định/user”
        Task ClearDefaultAsync(int userId, CancellationToken ct);
    }
}
