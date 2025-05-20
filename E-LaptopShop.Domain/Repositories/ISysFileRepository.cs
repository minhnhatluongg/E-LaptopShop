using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface ISysFileRepository
    {
        Task<SysFile> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<int> AddAsync(SysFile sysFile, CancellationToken cancellationToken = default);
        Task UpdateAsync(SysFile sysFile, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysFile>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    }
}
