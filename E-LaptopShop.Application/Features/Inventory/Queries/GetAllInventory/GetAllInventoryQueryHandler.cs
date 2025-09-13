using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryEntity = E_LaptopShop.Domain.Entities.Inventory;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory
{
    public class GetAllInventoryQueryHandler :
        BasePagedQueryHandler<InventoryEntity, InventoryDto, GetAllInventoryQuery>, 
        IRequestHandler<GetAllInventoryQuery, PagedResult<InventoryDto>>
    {
        private readonly IInventoryRepository _inventoryRepository;

        public GetAllInventoryQueryHandler(
            IMapper mapper, 
            ILogger<GetAllInventoryQuery> logger,
            IInventoryRepository inventory) : base(mapper, logger)
        {
            _inventoryRepository = inventory;
        }

        public Task<PagedResult<InventoryDto>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            return ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<InventoryEntity> ApplyDatabaseSearch(IQueryable<InventoryEntity> queryable, SearchOptions search)
        {
            if(!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!;
            return queryable.Where(i =>
                EF.Functions.Like(i.Product.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(i.Product.Description ?? "", $"%{searchTerm}%") ||
                EF.Functions.Like(i.Product.Category.Name ?? "", $"%{searchTerm}%") ||
                i.Product.ProductSpecifications.Any(spec =>
                    EF.Functions.Like(spec.CPU ?? "", $"%{searchTerm}%") ||
                    EF.Functions.Like(spec.RAM ?? "", $"%{searchTerm}%"))
            );
        }

        protected override IQueryable<InventoryEntity> ApplyDatabaseSorting(IQueryable<InventoryEntity> queryable, SortingOptions sort)
        {
            return sort.SortBy?.ToLowerInvariant() switch
            {
                "name" => sort.IsAscending 
                    ? queryable.OrderBy(i => i.Product.Name) : queryable.OrderByDescending(i => i.Product.Name),
                "price" => sort.IsAscending 
                    ? queryable.OrderBy(i => i.Product.Price) : queryable.OrderByDescending(i => i.Product.Price),
                "id" => sort.IsAscending 
                    ? queryable.OrderBy(i => i.Id) : queryable.OrderByDescending(i => i.Id),
                _ => queryable 
            };
        }

        protected override Task<IQueryable<InventoryEntity>> GetFilteredQueryable(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            var q = _inventoryRepository.GetFilteredQueryable(
                id: request.Id,
                productId: request.ProductId,
                currentStock: request.CurrentStock,
                minimumStock: request.MinimumStock,
                reorderPoint: request.ReorderPoint,
                averageCost: request.AverageCost,
                lastPurchasePrice: request.LastPurchasePrice,
                lastUpdated: request.LastUpdated,
                location: request.Location,
                status: request.Status
            );
            if (request.LowStockOnly == true)
                q = q.Where(i => i.CurrentStock <= i.MinimumStock);

            if (request.OutOfStockOnly == true)
                q = q.Where(i => i.CurrentStock == 0);

            return Task.FromResult(q);
        }
    }
}