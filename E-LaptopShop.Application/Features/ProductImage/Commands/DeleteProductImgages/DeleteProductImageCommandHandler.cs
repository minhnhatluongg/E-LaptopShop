using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.DeleteProductImgages
{
    /// <summary>
    /// Thin handler that delegates DeleteProductImage operations to ProductImageService
    /// Uses BaseService v2 pattern for clean separation of concerns and business logic encapsulation
    /// </summary>
    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, int>
    {
        private readonly IProductImageService _productImageService;
        private readonly ILogger<DeleteProductImageCommandHandler> _logger;

        public DeleteProductImageCommandHandler(
            IProductImageService productImageService,
            ILogger<DeleteProductImageCommandHandler> logger)
        {
            _productImageService = productImageService;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProductImageCommand for ProductImage ID: {ImageId}", request.Id);

            var result = await _productImageService.DeleteAsync(request.Id, cancellationToken);
            
            return result ? request.Id : throw new InvalidOperationException($"Failed to delete product image with ID {request.Id}");
        }
    }
}
