using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            return await ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<Product> ApplyDatabaseSearch(IQueryable<Product> queryable, SearchOptions search)
        {
            if(!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!;
            return queryable.Where(p =>
                EF.Functions.Like(p.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(p.Description ?? "", $"%{searchTerm}%") ||
                EF.Functions.Like(p.Category.Name ?? "", $"%{searchTerm}%") ||
                p.ProductSpecifications.Any(spec =>
                    EF.Functions.Like(spec.CPU ?? "", $"%{searchTerm}%") ||
                    EF.Functions.Like(spec.RAM ?? "", $"%{searchTerm}%"))
            );
        }

        protected override IQueryable<Product> ApplyDatabaseSorting(IQueryable<Product> queryable, SortingOptions sort)
        {
            return sort?.HasSorting.ToString().ToLowerInvariant() switch
            {
                "name" => sort.IsAscending
                    ? queryable.OrderBy(p => p.Name)
                    : queryable.OrderByDescending(p => p.Name),
                "price" => sort.IsAscending
                    ? queryable.OrderBy(p => p.Price)
                    : queryable.OrderByDescending(p => p.Price),
                "createdat" => sort.IsAscending
                    ? queryable.OrderBy(p => p.CreatedAt)
                    : queryable.OrderByDescending(p => p.CreatedAt),
                _ => queryable 
            };
        }

        protected override Task<IQueryable<Product>> GetFilteredQueryable(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var q = _productRepository.GetFilteredQueryable(
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.InStock);
            return Task.FromResult(q);
        }
    }
}
