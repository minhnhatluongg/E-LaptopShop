using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IBrandService : IBaseService<BrandDto, CreateBrandRequestDto, UpdateBrandRequestDto, BrandQueryParams>
    {
        Task<IEnumerable<BrandDto>> GetActiveBrandsAsync(CancellationToken cancellationToken = default);
    }
}
