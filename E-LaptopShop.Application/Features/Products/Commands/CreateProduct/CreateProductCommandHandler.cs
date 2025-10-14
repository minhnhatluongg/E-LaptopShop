using System.Threading;
using System.Threading.Tasks;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductService _productService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductService productService,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateProductCommand for product: {ProductName}", request.RequestDto.Name);

        return await _productService.CreateProductAsync(request.RequestDto, cancellationToken);
    }
} 