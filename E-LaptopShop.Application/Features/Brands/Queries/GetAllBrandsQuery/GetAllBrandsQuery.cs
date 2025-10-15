using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Queries.GetAllBrandsQuery
{
    public class GetAllBrandsQuery : BasePagedQuery<BrandDto>
    {
        public BrandQueryParams QueryParams { get; init; } = new BrandQueryParams();
    }
}
