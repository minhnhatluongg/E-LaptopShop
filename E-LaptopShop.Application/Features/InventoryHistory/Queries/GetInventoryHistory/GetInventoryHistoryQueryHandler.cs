using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
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
            return await ProcessQuery(request, cancellationToken);
        }

        // ✨ Implement abstract methods
        protected override async Task<IEnumerable<InventoryHistoryEntity>> GetFilteredEntities(GetInventoryHistoryQuery request, CancellationToken cancellationToken)
        {
            // Priority-based filtering (giữ logic cũ)
            if (request.ProductId.HasValue)
            {
                return await _historyRepository.GetTransactionHistoryAsync(
                    request.ProductId.Value, request.FromDate, request.ToDate);
            }
            else if (request.InventoryId.HasValue)
            {
                var histories = await _historyRepository.GetByInventoryIdAsync(request.InventoryId.Value);
                return ApplyDateRangeFilter(histories, request.FromDate, request.ToDate);
            }
            else if (!string.IsNullOrEmpty(request.TransactionType))
            {
                if (Enum.TryParse<InventoryTransactionType>(request.TransactionType, out var transactionType))
                {
                    var histories = await _historyRepository.GetByTransactionTypeAsync(transactionType);
                    return ApplyDateRangeFilter(histories, request.FromDate, request.ToDate);
                }
                return new List<InventoryHistoryEntity>();
            }
            else if (request.FromDate.HasValue || request.ToDate.HasValue)
            {
                return await _historyRepository.GetByDateRangeAsync(
                    request.FromDate ?? DateTime.MinValue,
                    request.ToDate ?? DateTime.MaxValue);
            }
            else if (!string.IsNullOrEmpty(request.ReferenceType) && request.ReferenceId.HasValue)
            {
                return await _historyRepository.GetByReferenceAsync(request.ReferenceType, request.ReferenceId.Value);
            }

            return await _historyRepository.GetAllAsync();
        }

        protected override IEnumerable<InventoryHistoryEntity> ApplySearch(IEnumerable<InventoryHistoryEntity> entities, SearchOptions search)
        {
            // ✨ Sử dụng SearchHelper generic với custom fields
            var customSearchFields = new[] { "TransactionType", "Notes", "CreatedBy", "ReferenceType" };
            
            var searchResult = SearchHelper.ApplyGenericSearch(entities, search, customSearchFields);

            // ✨ Custom search cho navigation properties
            if (search.HasSearch && search.SearchFields?.Contains("ProductName") == true)
            {
                var searchTerm = search.SearchTerm!.ToLower();
                searchResult = searchResult.Where(h => 
                    h.Inventory?.Product?.Name?.ToLower().Contains(searchTerm) == true);
            }

            return searchResult;
        }

        protected override IEnumerable<InventoryHistoryEntity> ApplySorting(IEnumerable<InventoryHistoryEntity> entities, SortingOptions sort)
        {
            // ✨ Custom sort mappings cho calculated fields và navigation properties
            var sortMappings = new Dictionary<string, Func<InventoryHistoryEntity, object>>
            {
                ["transactiondate"] = h => h.TransactionDate,
                ["transactiontype"] = h => h.TransactionType ?? "",
                ["quantity"] = h => h.Quantity,
                ["unitcost"] = h => h.UnitCost,
                ["totalvalue"] = h => h.Quantity * h.UnitCost,
                ["productname"] = h => h.Inventory?.Product?.Name ?? "",
                ["createdby"] = h => h.CreatedBy ?? ""
            };

            return SortHelper.ApplyCustomSorting(entities, sort, sortMappings);
        }

        protected override IEnumerable<InventoryHistoryEntity> ApplyDefaultSorting(IEnumerable<InventoryHistoryEntity> entities)
        {
            return entities.OrderByDescending(h => h.TransactionDate);
        }

        // Helper methods
        private IEnumerable<InventoryHistoryEntity> ApplyDateRangeFilter(IEnumerable<InventoryHistoryEntity> histories, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue)
                histories = histories.Where(h => h.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                histories = histories.Where(h => h.TransactionDate <= toDate.Value);

            return histories;
        }
    }
}
