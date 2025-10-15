using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(
        IMapper mapper,
        IProductService productService,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _productService = productService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllProductsQuery - Search: {Search}, CategoryId: {CategoryId}",
            request.Search, request.CategoryId);

        var queryParams= _mapper.Map<ProductQueryParams>(request);
        queryParams.ValidateAndNormalize();
        queryParams.ValidateBusinessRules();
        
        return await _productService.GetAllProductsAsync(queryParams, cancellationToken);
    }
}