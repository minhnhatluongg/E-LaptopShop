using E_LaptopShop.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Base
{
    public interface IReadOnlyService <TDto, in TQueryParams>
    {
        Task<TDto?> GetByIdAsync(int id , CancellationToken cancellationToken = default);
        Task<PagedResult<TDto>> GetAllAsync(TQueryParams queryParams, CancellationToken cancellationToken = default);
    }
}
