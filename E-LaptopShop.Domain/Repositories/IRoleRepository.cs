using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<Role> ChangeActiveAsync (int id, bool isActive, CancellationToken cancellationToken);
        Task<IEnumerable<Role>> GetFilteredAsync(int? id, string? name, bool? isActive, CancellationToken cancellationToken);
    }
}
