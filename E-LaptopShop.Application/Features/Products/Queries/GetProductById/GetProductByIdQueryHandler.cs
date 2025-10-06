using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    private readonly IProductService _productService;

    public GetProductByIdQueryHandler(IProductService productService, ILogger<GetProductByIdQueryHandler> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Throw.IfNullOrNonPositive(request.Id, nameof(request.Id));
        _logger.LogInformation("Handling GetProductByIdQuery - Id: {Id}", request.Id);
        var product = await _productService.GetProductByIdAsync(request.Id, cancellationToken);
        return product;
    }
} 