using E_LaptopShop.Application.Common.Helpers;  
using E_LaptopShop.Application.Common.Pagination; 
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System.Linq;

public class BrandQueryParams : PaginationParams
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }

    public string? SortBy { get; set; }
    public bool IsAscending { get; set; } = true;

    public SortingOptions SortingOptions => new SortingOptions
    {
        SortBy = SortBy,
        IsAscending = IsAscending
    };
    public void ValidateAndNormalize()
    {
        if (PageNumber <= 0) PageNumber = 1;
        if (PageSize <= 0) PageSize = 10;

        if (!string.IsNullOrEmpty(Name))
        {
            Name = Name.Trim();
            if (string.IsNullOrEmpty(Name)) Name = null;
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