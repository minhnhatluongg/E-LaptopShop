using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IRoleLookup
    {
        Task<int> GetIdByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<string> GetNameByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
