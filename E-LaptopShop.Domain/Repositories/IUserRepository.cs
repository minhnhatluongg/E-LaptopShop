using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> AddAsync(User user, CancellationToken cancellationToken);
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<User> ChangeActiveAsync(int id, bool isActive, CancellationToken cancellationToken);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFilteredAsync(UserFilterParams filterParams, CancellationToken cancellationToken);
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, 
            UserFilterParams filterParams, 
            CancellationToken cancellationToken);
    }
}
