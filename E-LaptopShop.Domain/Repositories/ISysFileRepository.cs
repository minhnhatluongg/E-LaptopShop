using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    /// <summary>
    /// Lightweight repository contract for SysFile.
    /// NOTE: Intentionally NOT inheriting IBaseRepository to keep the surface
    /// narrow (Interface Segregation Principle) — SysFile lifecycle is simpler
    /// than other aggregates.
    /// </summary>
    public interface ISysFileRepository
    {
        Task<SysFile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysFile>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        Task<SysFile> AddAsync(SysFile sysFile, CancellationToken cancellationToken = default);
        Task UpdateAsync(SysFile sysFile, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
