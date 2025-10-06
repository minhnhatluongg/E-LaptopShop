using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Base
{
    public interface IBaseService<TDto, in TCreateDto, in TUpdateDto, in TQueryParams> 
        : IReadOnlyService<TDto, TQueryParams>, IWriteOnlyService<TDto, TCreateDto, TUpdateDto>
    {

    }
}
