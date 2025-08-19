using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : 
        BasePagedQueryHandler<Product, ProductDto, GetProductsQuery>,
        IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsQueryHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<GetProductsQueryHandler> logger) : base(mapper, logger)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await ProcessQuery(request, cancellationToken);
        }

        protected override async Task<IEnumerable<Product>> GetFilteredEntities(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetFilteredAsync(
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.InStock,
                cancellationToken);

            if (request.IsActive.HasValue)
            {
                products = products.Where(p => p.IsActive == request.IsActive.Value);
            }

            return products;
        }

        protected override IEnumerable<Product> ApplySearch(IEnumerable<Product> entities, SearchOptions search)
        {
            // Default search trong Name, Description
            return SearchHelper.ApplyGenericSearch(entities, search, new[] { "Name", "Description" });
        }

        protected override IEnumerable<Product> ApplySorting(IEnumerable<Product> entities, SortingOptions sort)
        {
            var sortMappings = new Dictionary<string, Func<Product, object>>
            {
                ["name"] = p => p.Name,
                ["price"] = p => p.Price,
                ["createdat"] = p => p.CreatedAt,
                ["categoryname"] = p => p.Category?.Name ?? ""
            };

            return SortHelper.ApplyCustomSorting(entities, sort, sortMappings);
        }
    }
}
