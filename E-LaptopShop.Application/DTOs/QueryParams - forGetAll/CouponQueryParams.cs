using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;

public class CouponQueryParams : PaginationParams
{
    public string? Code { get; set; }
    public string? DiscountType { get; set; }
    public bool? IsActive { get; set; }
    public bool? OnlyAvailable { get; set; } // active + not expired + has remaining usage
    public DateTime? StartsFrom { get; set; }
    public DateTime? EndsBefore { get; set; }

    /// <summary>Global search across Code + Description.</summary>
    public string? Search { get; set; }

    public string? SortBy { get; set; }
    public bool IsAscending { get; set; } = false;

    public SortingOptions SortingOptions => new SortingOptions
    {
        SortBy = SortBy,
        IsAscending = IsAscending
    };

    public void ValidateAndNormalize()
    {
        if (PageNumber <= 0) PageNumber = 1;
        if (PageSize <= 0) PageSize = 10;

        if (!string.IsNullOrEmpty(Code))
        {
            Code = Code.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(Code)) Code = null;
        }

        if (!string.IsNullOrEmpty(DiscountType))
        {
            DiscountType = DiscountType.Trim().ToLowerInvariant();
        }

        if (!string.IsNullOrEmpty(SortBy))
        {
            SortBy = SortBy.Trim().ToLowerInvariant();
        }
    }

    public void ValidateBusinessRules()
    {
    }
}
