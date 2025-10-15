using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;

namespace E_LaptopShop.Application.DTOs.QueryParams
{
    public class ProductQueryParams : PaginationParams
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public bool? IsActive { get; set; }
        public decimal? MinDiscount { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int? BrandId { get; set; }

        // Search parameters
        public string? Search { get; set; }

        // Sorting parameters  
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;

        // Helper properties for SearchHelper and SortHelper
        public SearchOptions SearchOptions => new SearchOptions
        {
            SearchTerm = Search,
            SearchFields = null 
        };

        public SortingOptions SortingOptions => new SortingOptions
        {
            SortBy = SortBy,
            IsAscending = IsAscending
        };

        public void ValidateAndNormalize()
        {
            if (PageNumber <= 0) PageNumber = 1;
            if (PageSize <= 0) PageSize = 10;

            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            {
                (MinPrice, MaxPrice) = (MaxPrice, MinPrice);
            }

            if (!string.IsNullOrEmpty(Search))
            {
                Search = Search.Trim();
                if (string.IsNullOrEmpty(Search)) Search = null;
            }

            if (!string.IsNullOrEmpty(SortBy))
            {
                SortBy = SortBy.Trim().ToLowerInvariant();
            }

            if(MinDiscount.HasValue && MaxDiscount.HasValue && MinDiscount > MaxDiscount)
            {
                (MinDiscount, MaxDiscount) = (MaxDiscount, MinDiscount);
            }
        }
        public void ValidateBusinessRules()
        {
            if (MinPrice.HasValue && MinPrice < 0)
                throw new ArgumentException("Minimum price cannot be negative");

            if (MaxPrice.HasValue && MaxPrice < 0)
                throw new ArgumentException("Maximum price cannot be negative");

            if (MinDiscount.HasValue && MinDiscount < 0)
                throw new ArgumentException("Minimum discount cannot be negative");

            if (MaxDiscount.HasValue && MaxDiscount > 100)
                throw new ArgumentException("Maximum discount cannot exceed 100%");
        }
    }
}
