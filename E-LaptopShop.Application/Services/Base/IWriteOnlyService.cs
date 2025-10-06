using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Base
{
    public interface IWriteOnlyService <TDto,in TCreateDto, in TUpdateDto>
    {
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
        Task<TDto> UpdateAsync(int id,TUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
