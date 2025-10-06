using E_LaptopShop.Application.Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using System;

namespace E_LaptopShop.Application.DTOs.QueryParams
{
    public class ProductImageQueryParams : PaginationParams
    {
        [FromQuery(Name = "id")]
        public int? Id { get; init; }

        [FromQuery(Name = "productId")]
        public int? ProductId { get; init; }

        [FromQuery(Name = "imageUrl")]
        public string? ImageUrl { get; init; }

        [FromQuery(Name = "isMain")]
        public bool? IsMain { get; init; }

        [FromQuery(Name = "fileType")]
        public string? FileType { get; init; }

        [FromQuery(Name = "minFileSize")]
        public long? MinFileSize { get; init; }

        [FromQuery(Name = "maxFileSize")]
        public long? MaxFileSize { get; init; }

        [FromQuery(Name = "displayOrder")]
        public int? DisplayOrder { get; init; }

        [FromQuery(Name = "altText")]
        public string? AltText { get; init; }

        [FromQuery(Name = "title")]
        public string? Title { get; init; }

        [FromQuery(Name = "createdAfter")]
        public DateTime? CreatedAfter { get; init; }

        [FromQuery(Name = "createdBefore")]
        public DateTime? CreatedBefore { get; init; }

        [FromQuery(Name = "uploadedAfter")]
        public DateTime? UploadedAfter { get; init; }

        [FromQuery(Name = "uploadedBefore")]
        public DateTime? UploadedBefore { get; init; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; init; }

        [FromQuery(Name = "createdBy")]
        public string? CreatedBy { get; init; }

        public string? Search { get; set; }

        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;

        public void ValidateAndNormalize()
        {
            if (PageNumber <= 0) PageNumber = 1;
            if (PageSize <= 0) PageSize = 10;

            if (!string.IsNullOrEmpty(Search))
            {
                Search = Search.Trim();
                if (string.IsNullOrEmpty(Search)) Search = null;
            }

            if (!string.IsNullOrEmpty(SortBy))
            {
                SortBy = SortBy.Trim().ToLowerInvariant();
            }
        }

        public void ValidateBusinessRules()
        {
            if (MinFileSize.HasValue && MinFileSize < 0)
                throw new ArgumentException("Minimum file size cannot be negative");

            if (MaxFileSize.HasValue && MaxFileSize < 0)
                throw new ArgumentException("Maximum file size cannot be negative");

            // Display order must be non-negative
            if (DisplayOrder.HasValue && DisplayOrder < 0)
                throw new ArgumentException("Display order cannot be negative");

            if (ProductId.HasValue && ProductId <= 0)
                throw new ArgumentException("Product ID must be greater than zero");
        }

        public bool HasSearchCriteria()
        {
            return
                   !string.IsNullOrWhiteSpace(ImageUrl) ||
                   !string.IsNullOrWhiteSpace(AltText) ||
                   !string.IsNullOrWhiteSpace(Title) ||
                   !string.IsNullOrWhiteSpace(CreatedBy);
        }

        public bool HasFilterCriteria()
        {
            return Id.HasValue ||
                   ProductId.HasValue ||
                   IsMain.HasValue ||
                   IsActive.HasValue ||
                   !string.IsNullOrWhiteSpace(FileType) ||
                   MinFileSize.HasValue ||
                   MaxFileSize.HasValue ||
                   DisplayOrder.HasValue ||
                   CreatedAfter.HasValue ||
                   CreatedBefore.HasValue ||
                   UploadedAfter.HasValue ||
                   UploadedBefore.HasValue;
        }
    }
}
