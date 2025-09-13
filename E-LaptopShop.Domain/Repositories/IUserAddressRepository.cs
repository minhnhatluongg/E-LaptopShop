using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IUserAddressRepository
    {
        IQueryable<UserAddress> GetFilteredQueryable(UserAddressFilterParams filter, bool includeUser = false);
        IQueryable<UserAddress> Query(); // Lọc isDeleted = false
        IQueryable<UserAddress> QueryIgnoreFilters(); //Lấy kể soft delete

        Task<UserAddress?> FindAsync(int id, int userId, CancellationToken ct);
        Task<UserAddress?> FindByIdAsync(int id, CancellationToken ct);
        Task<int> UnsetDefaultForUserAsync(int userId, int skipId, CancellationToken ct);
        EntityEntry<UserAddress> Entry(UserAddress entity); 
        Task AddAsync(UserAddress entity, CancellationToken ct);
        Task UpdateAsync(UserAddress entity, CancellationToken ct);
        Task<int> HardDeleteAsync(int id, CancellationToken ct);
        Task<int> DeleteAsync (int id, CancellationToken ct);

        Task<int> SaveChangesAsync(CancellationToken ct);

        // Hỗ trợ rule “1 mặc định/user”
        Task ClearDefaultAsync(int userId, CancellationToken ct);
    }
}
