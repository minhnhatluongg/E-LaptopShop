using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, BrandDto>
    {
        private readonly IBrandService _brandService;

        public UpdateBrandCommandHandler(
            IBrandService brandService
            )
        {
            _brandService = brandService;
        }
        public async Task<BrandDto> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            return await _brandService.UpdateAsync(request.Id, request.RequestDto, cancellationToken);
        }
    }
}
