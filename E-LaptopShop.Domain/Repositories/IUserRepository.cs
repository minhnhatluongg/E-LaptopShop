using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> ChangeActiveAsync(int id, bool isActive, CancellationToken cancellationToken);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFilteredAsync(UserFilterParams filterParams, CancellationToken cancellationToken);
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, 
            UserFilterParams filterParams, 
            CancellationToken cancellationToken);

        Task IncrementLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
        Task ResetLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
        Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default);
    }
}
