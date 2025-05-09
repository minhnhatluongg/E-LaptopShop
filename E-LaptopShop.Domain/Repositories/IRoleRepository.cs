using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
        Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Role> AddAsync(Role role, CancellationToken cancellationToken);
        Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<Role> ChangeActiveAsync (int id, bool isActive, CancellationToken cancellationToken);
        Task<IEnumerable<Role>> GetFilteredAsync(int? id, string? name, bool? isActive, CancellationToken cancellationToken);
    }
}
