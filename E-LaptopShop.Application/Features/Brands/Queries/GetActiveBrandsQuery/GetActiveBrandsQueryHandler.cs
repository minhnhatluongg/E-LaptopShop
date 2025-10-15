using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Queries.GetActiveBrandsQuery
{
    public class GetActiveBrandsQueryHandler : IRequestHandler<GetActiveBrandsQuery, IEnumerable<BrandDto>>
    {
        private readonly IBrandService _brandService;
        public GetActiveBrandsQueryHandler(IBrandService brandService)
        {
            _brandService = brandService;
        }
        public async Task<IEnumerable<BrandDto>> Handle(GetActiveBrandsQuery request, CancellationToken cancellationToken)
        {
            return await _brandService.GetActiveBrandsAsync(cancellationToken);
        }
    }
}
