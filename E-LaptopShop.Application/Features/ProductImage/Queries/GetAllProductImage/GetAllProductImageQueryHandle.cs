using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductImageEntity = E_LaptopShop.Domain.Entities.ProductImage;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetAllFilteredAndPagination
{
    public class GetAllProductImageQueryHandle : BasePagedQueryHandler<ProductImageEntity, ProductImageDto, GetAllProductImageQuery>,
        IRequestHandler<GetAllProductImageQuery, PagedResult<ProductImageDto>>
    {
        private readonly IProductImageRepository _productImageRepository;

        public GetAllProductImageQueryHandle(
            IMapper mapper,
            ILogger<GetAllProductImageQueryHandle> logger,
            IProductImageRepository productImageRepository)
            : base(mapper, logger)
        {
            _productImageRepository = productImageRepository;
        }

        public Task<PagedResult<ProductImageDto>> Handle(GetAllProductImageQuery request, CancellationToken cancellationToken)
        {
            return ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<ProductImageEntity> ApplyDatabaseSearch(IQueryable<ProductImageEntity> queryable, SearchOptions search)
        {
            if (!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!;

            int? searchInt = null;

            if (int.TryParse(searchTerm, out int parsedInt))
                searchInt = parsedInt;

            return queryable.Where(img =>
                // Search trong ImageUrl
                EF.Functions.Like(img.ImageUrl, $"%{searchTerm}%") ||
                // Search trong Caption (nếu có)
                (img.AltText != null && EF.Functions.Like(img.AltText, $"%{searchTerm}") ||
                img.Title != null && EF.Functions.Like(img.Title, $"%{searchTerm}") ||
                searchInt != null && img.DisplayOrder == searchInt)
             );
        }

        protected override IQueryable<ProductImageEntity> ApplyDatabaseSorting(IQueryable<ProductImageEntity> queryable, SortingOptions sort)
        {
            return sort?.SortBy.ToLowerInvariant() switch
            {
                "imageurl" => sort.IsAscending
                    ? queryable.OrderBy(img => img.ImageUrl)
                    : queryable.OrderByDescending(img => img.ImageUrl),
                "isactive" => sort.IsAscending
                    ? queryable.OrderBy(img => img.IsActive)
                    : queryable.OrderByDescending(img => img.IsActive),
                "ismain" => sort.IsAscending
                    ? queryable.OrderBy(img => img.IsMain)
                    : queryable.OrderByDescending(img => img.IsMain),
                "displayorder" => sort.IsAscending
                    ? queryable.OrderBy(img => img.DisplayOrder)
                    : queryable.OrderByDescending(img => img.DisplayOrder),
                _ => ApplyDefaultDatabaseSorting(queryable) // Mặc định nếu không khớp
            };
        }

        protected override Task<IQueryable<ProductImageEntity>> GetFilteredQueryable(GetAllProductImageQuery request, CancellationToken cancellationToken)
        {
            var q = _productImageRepository.GetFilteredQueryable (new ProductImageFilterParams
            {
                Id = request.Id,
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl,
                IsMain = request.IsMain,
                DisplayOrder = request.DisplayOrder,
                Title = request.Title,
                CreatedAt = request.CreatedAt,
                UploadedAt = request.UploadedAt,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy
            });
            return Task.FromResult(q);
        }
    }
}
