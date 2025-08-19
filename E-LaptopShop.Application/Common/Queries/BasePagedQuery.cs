using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using MediatR;

namespace E_LaptopShop.Application.Common.Queries
{
    /// <summary>
    /// Base class cho tất cả paged queries
    /// </summary>
    public abstract class BasePagedQuery<TResponse> : PaginationParams, IRequest<PagedResult<TResponse>>
    {
        // Search options
        public SearchOptions Search { get; set; } = new();
        
        // Sorting options  
        public SortingOptions Sort { get; set; } = new();
    }
}
