using System.Threading;
using System.Threading.Tasks;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Optimized handler using BaseService pattern
/// Handler responsibility: Extract DTO and delegate to service
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductService _productService;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductService productService,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateProductCommand for product ID: {ProductId}", request.RequestDto.Id);

        // Use BaseService method - all validation, business rules, and mapping handled automatically
        return await _productService.UpdateProductAsync(request.RequestDto, cancellationToken);
    }
} 