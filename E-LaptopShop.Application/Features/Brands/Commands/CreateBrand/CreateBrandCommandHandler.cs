using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDto>
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<CreateBrandCommandHandler> _logger;

        public CreateBrandCommandHandler(IBrandService brandService, ILogger<CreateBrandCommandHandler> logger)
        {
            brandService = _brandService;
            _logger = logger;
        }
        public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateBrandCommand for brand: {BrandName}", request.RequestDto.Name);
            if (request.RequestDto == null)
            {
                _logger.LogError("CreateBrandCommand received with null RequestDto.");
                throw new ArgumentNullException(nameof(request.RequestDto), "Request data cannot be null.");
            }

            var createdBrand = await _brandService.CreateAsync(
                request.RequestDto,
                cancellationToken);

            _logger.LogInformation("Brand created successfully with ID: {Id}", createdBrand.Id);

            return createdBrand;
        }
    }
}
