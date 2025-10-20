using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface ISysFileRepository : IBaseRepository<SysFile>
    {
        Task<IEnumerable<SysFile>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    }
}
