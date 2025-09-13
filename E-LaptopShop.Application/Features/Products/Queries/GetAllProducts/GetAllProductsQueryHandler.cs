using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Domain.Entities;
using Microsoft.Extensions.Logging;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : BasePagedQueryHandler<Product, ProductDto, GetAllProductsQuery>, IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetAllProductsQueryHandler> logger) : base(mapper, logger)
    {
        _productRepository = productRepository;
    }

    protected override IQueryable<Product> ApplyDatabaseSearch(IQueryable<Product> queryable, SearchOptions search)
    {
        if (!search.HasSearch) return queryable;
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

    protected override IQueryable<Product> ApplyDatabaseSorting(IQueryable<Product> q, SortingOptions sort)
    {
        return sort.SortBy?.ToLowerInvariant() switch
        {
            "name" => sort.IsAscending ? q.OrderBy(p => p.Name) : q.OrderByDescending(p => p.Name),
            "price" => sort.IsAscending ? q.OrderBy(p => p.Price) : q.OrderByDescending(p => p.Price),
            "id" => sort.IsAscending ? q.OrderBy(p => p.Id) : q.OrderByDescending(p => p.Id),
            _ => ApplyDefaultDatabaseSorting(q)
        };
    }

    protected override Task<IQueryable<Product>> GetFilteredQueryable(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var min = request.MinPrice;
        var max = request.MaxPrice;
        if (min.HasValue && max.HasValue && min > max)
            (min, max) = (max, min);

        var q = _productRepository.GetFilteredQueryable(request.CategoryId, min, max, request.InStock);
        return Task.FromResult(q);
    }

    Task<PagedResult<ProductDto>> IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>.Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return ProcessQueryOptimized(request, cancellationToken);
    }

}