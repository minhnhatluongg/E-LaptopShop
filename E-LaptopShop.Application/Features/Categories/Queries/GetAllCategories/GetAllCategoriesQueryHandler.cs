using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoriEntity = E_LaptopShop.Domain.Entities.Category;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : BasePagedQueryHandler<CategoriEntity, CategoryDto, GetAllCategoriesQuery>, IRequestHandler<GetAllCategoriesQuery, PagedResult<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetAllCategoriesQueryHandler(
            IMapper mapper, 
            ILogger<GetAllCategoriesQueryHandler> logger,
            ICategoryRepository categoryRepository) : base(mapper, logger)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<PagedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<CategoriEntity> ApplyDatabaseSearch(IQueryable<CategoriEntity> queryable, SearchOptions search)
        {
            if (!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!;

            return queryable.Where(c =>
                EF.Functions.Like(c.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(c.Description ?? "", $"%{searchTerm}%"));
        }

        protected override IQueryable<CategoriEntity> ApplyDatabaseSorting(IQueryable<CategoriEntity> queryable, SortingOptions sort)
        {
            return sort.SortBy?.ToLowerInvariant() switch
            {
                "name" => sort.IsAscending ? queryable.OrderBy(c => c.Name) : queryable.OrderByDescending(c => c.Name),
                "id" => sort.IsAscending ? queryable.OrderBy(c => c.Id) : queryable.OrderByDescending(c => c.Id),
                _ => queryable 
            };
        }

        protected override Task<IQueryable<CategoriEntity>> GetFilteredQueryable(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var q = _categoryRepository.GetFilteredQueryable(
                  id: request.Id,
                  name: request.Name,
                  description: request.Description);
            return Task.FromResult(q);
        }
    }
}
