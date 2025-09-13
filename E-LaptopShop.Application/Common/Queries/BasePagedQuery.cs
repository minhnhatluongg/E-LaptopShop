using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace E_LaptopShop.Application.Common.Queries
{
    public abstract class BasePagedQuery<TResponse> : PaginationParams, IRequest<PagedResult<TResponse>>
    {
        /// <summary>
        /// Global search term - searches across multiple fields intelligently
        /// </summary>
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        /// <summary>
        /// Sort field name (e.g: name, price, createdat)
        /// </summary>
        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction: true = ascending, false = descending
        /// </summary>
        [FromQuery(Name = "isAscending")]
        public bool IsAscending { get; set; } = true;

        [JsonIgnore]
        internal SearchOptions SearchOptions => new SearchOptions
        {
            SearchTerm = Search,
            SearchFields = null // Smart search across all relevant fields
        };

        [JsonIgnore]
        internal SortingOptions SortOptions => new SortingOptions
        {
            SortBy = SortBy,
            IsAscending = IsAscending
        };
    }
}