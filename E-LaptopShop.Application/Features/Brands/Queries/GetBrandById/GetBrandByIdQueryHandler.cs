using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Queries.GetBrandById
{
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto>
    {
        private readonly IBrandService _brandService;
        public GetBrandByIdQueryHandler(IBrandService brandService)
        {
            _brandService = brandService;
        }
        public async Task<BrandDto> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            return await _brandService.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
