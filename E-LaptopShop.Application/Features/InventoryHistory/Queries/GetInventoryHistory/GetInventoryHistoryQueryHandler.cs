using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InventoryHistoryEntity = E_LaptopShop.Domain.Entities.InventoryHistory;

namespace E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryHistory
{
    public class GetInventoryHistoryQueryHandler :
        BasePagedQueryHandler<InventoryHistoryEntity, InventoryHistoryDto, GetInventoryHistoryQuery>,
        IRequestHandler<GetInventoryHistoryQuery, PagedResult<InventoryHistoryDto>>
    {
        private readonly IInventoryHistoryRepository _historyRepository;

        public GetInventoryHistoryQueryHandler(
            IInventoryHistoryRepository historyRepository,
            IMapper mapper,
            ILogger<GetInventoryHistoryQueryHandler> logger) : base(mapper, logger)
        {
            _historyRepository = historyRepository;
        }

        public async Task<PagedResult<InventoryHistoryDto>> Handle(GetInventoryHistoryQuery request, CancellationToken cancellationToken)
        {
            return await ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<InventoryHistoryEntity> ApplyDatabaseSearch(IQueryable<InventoryHistoryEntity> queryable, SearchOptions search)
        {
            if (!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!;
            return queryable.Where(h =>
                // Search trong TransactionType (string property)
                EF.Functions.Like(h.TransactionType, $"%{searchTerm}%") ||

                // Search trong Notes (nếu có)
                (h.Notes != null && EF.Functions.Like(h.Notes, $"%{searchTerm}%")) ||

                // Search trong CreatedBy (nếu có)  
                (h.CreatedBy != null && EF.Functions.Like(h.CreatedBy, $"%{searchTerm}%")) ||

                // Search trong Product Name (qua Inventory navigation)
                EF.Functions.Like(h.Inventory.Product.Name, $"%{searchTerm}%") ||
                // Search trong ReferenceType (nếu có)
                (h.ReferenceType != null && EF.Functions.Like(h.ReferenceType, $"%{searchTerm}%"))
             );
        }

        protected override IQueryable<InventoryHistoryEntity> ApplyDatabaseSorting(IQueryable<InventoryHistoryEntity> queryable, SortingOptions sort)
        {
            return sort.SortBy?.ToLowerInvariant() switch
            {
                "transactiontype" => sort.IsAscending
                    ? queryable.OrderBy(h => h.TransactionType)
                    : queryable.OrderByDescending(h => h.TransactionType),
                "id" => sort.IsAscending
                    ? queryable.OrderBy(h => h.Id)
                    : queryable.OrderByDescending(h => h.Id),
                _ => queryable // Dùng mặc định (Id desc)
            };
        }

        protected override Task<IQueryable<InventoryHistoryEntity>> GetFilteredQueryable(GetInventoryHistoryQuery request, CancellationToken cancellationToken)
        {
            var q = _historyRepository.GetFilteredQueryable(
                inventoryId: request.InventoryId,
                transactionType: request.TransactionType,
                referenceType: request.ReferenceType,
                fromDate: request.FromDate,
                toDate: request.ToDate);
            return Task.FromResult(q);
        }
    }
}
