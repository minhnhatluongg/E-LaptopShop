using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.DTOs.QueryParams___forGetAll
{
    public class CategoriesParams : PaginationParams
    {
        // ---- FILTERS ----
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;

        // Tìm kiếm tổng quát (áp dụng Name/Slug/Description)
        public string? Search { get; set; }

        // ---- RANGE (nếu entity có các trường thời gian) ----
        public DateTimeOffset? CreatedFrom { get; set; }
        public DateTimeOffset? CreatedTo { get; set; }
        public DateTimeOffset? UpdatedFrom { get; set; }
        public DateTimeOffset? UpdatedTo { get; set; }

        // ---- SORTING ----
        public string? SortBy { get; set; }          // "Id" | "Name" | "CreatedAt" | "UpdatedAt" | "ParentId"
        public bool IsAscending { get; set; } = false;

        public SortingOptions SortingOptions => new SortingOptions
        {
            SortBy = SortBy,
            IsAscending = IsAscending
        };

        // ---- NORMALIZE ----
        public void ValidateAndNormalize()
        {
            if (PageNumber <= 0) PageNumber = 1;
            if (PageSize <= 0) PageSize = 10;

            if (Id <= 0) Id = null;
            if (ParentId <= 0) ParentId = null;

            Name = NormalizeOrNull(Name);
            Slug = NormalizeOrNull(Slug);
            Description = NormalizeOrNull(Description);
            Search = NormalizeOrNull(Search);

            if (!string.IsNullOrWhiteSpace(SortBy))
                SortBy = SortBy.Trim();
        }

        // ---- BUSINESS RULES ----
        public void ValidateBusinessRules()
        {
            if (CreatedFrom.HasValue && CreatedTo.HasValue && CreatedFrom > CreatedTo)
                throw new ArgumentException("CreatedFrom must be <= CreatedTo");

            if (UpdatedFrom.HasValue && UpdatedTo.HasValue && UpdatedFrom > UpdatedTo)
                throw new ArgumentException("UpdatedFrom must be <= UpdatedTo");

            EnsureMaxLen(Name, 200, nameof(Name));
            EnsureMaxLen(Slug, 200, nameof(Slug));
            EnsureMaxLen(Description, 1000, nameof(Description));
            EnsureMaxLen(Search, 200, nameof(Search));
        }

        // ---- Helpers ----
        private static string? NormalizeOrNull(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var t = s.Trim();
            return t.Length == 0 ? null : t;
        }

        private static void EnsureMaxLen(string? s, int max, string paramName)
        {
            if (s != null && s.Length > max)
                throw new ArgumentException($"{paramName} length must be <= {max}");
        }
    }
}
