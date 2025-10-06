using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage
{
    /// <summary>
    /// Thin handler that delegates UpdateProductImage operations to ProductImageService
    /// Uses BaseService v2 pattern for clean separation of concerns and business logic encapsulation
    /// </summary>
    public class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand, ProductImageDto>
    {
        private readonly IProductImageService _productImageService;
        private readonly ILogger<UpdateProductImageCommandHandler> _logger;

        public UpdateProductImageCommandHandler(
            IProductImageService productImageService,
            ILogger<UpdateProductImageCommandHandler> logger)
        {
            _productImageService = productImageService;
            _logger = logger;
        }

        public async Task<ProductImageDto> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProductImageCommand for ProductImage ID: {ImageId}", request.Id);

            var updateDto = new UpdateProductImageRequestDto
            {
                Id = request.Id,
                ProductId = request.ProductId,
                SysFileId = request.SysFileId,
                ImageUrl = request.ImageUrl,
                FileType = request.FileType,
                FileSize = request.FileSize,
                IsMain = request.IsMain,
                DisplayOrder = request.DisplayOrder,
                AltText = request.AltText,
                Title = request.Title,
                IsActive = request.IsActive
            };
            return await _productImageService.UpdateAsync(request.Id, updateDto, cancellationToken);
        }
    }
}
