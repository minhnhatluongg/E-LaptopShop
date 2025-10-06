using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetAllProductImage
{
    public class GetAllProductImageQueryHandler : IRequestHandler<GetAllProductImageQuery, PagedResult<ProductImageDto>>
    {
        private readonly IProductImageService _productImageService;
        private readonly ILogger<GetAllProductImageQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetAllProductImageQueryHandler(
            IProductImageService productImageService,
            ILogger<GetAllProductImageQueryHandler> logger,
            IMapper mapper)
        {
            _productImageService = productImageService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductImageDto>> Handle(GetAllProductImageQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllProductImageQuery - ProductId: {ProductId}, Search: {Search}",
                request.ProductId, request.Search);
            var queryParams = _mapper.Map<GetAllProductImageQuery, ProductImageQueryParams>(request);
            queryParams.ValidateAndNormalize();
            queryParams.ValidateBusinessRules();
            return await _productImageService.GetAllAsync(queryParams, cancellationToken);
        }
    }
}
