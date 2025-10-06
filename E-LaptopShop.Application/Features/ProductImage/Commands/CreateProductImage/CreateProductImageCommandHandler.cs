using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage
{
    /// <summary>
    /// Thin handler that delegates CreateProductImage operations to ProductImageService
    /// Uses BaseService v2 pattern for clean separation of concerns and business logic encapsulation
    /// </summary>
    public class CreateProductImageCommandHandler : IRequestHandler<CreateProductImageCommand, ProductImageDto>
    {
        private readonly IProductImageService _productImageService;
        private readonly ILogger<CreateProductImageCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateProductImageCommandHandler(
            IMapper mapper,
            IProductImageService productImageService,
            ILogger<CreateProductImageCommandHandler> logger)
        {
            _productImageService = productImageService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ProductImageDto> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProductImageCommand for ProductId: {ProductId}", request.ProductId);
            var createDto = _mapper.Map<CreateProductImageCommand, CreateProductImageRequestDto>(request);
            return await _productImageService.CreateAsync(createDto, cancellationToken);
        }
    }
}
