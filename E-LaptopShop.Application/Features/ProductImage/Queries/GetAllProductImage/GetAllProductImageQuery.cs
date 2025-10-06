using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetAllProductImage
{
    /// <summary>
    /// Query for getting all ProductImages with comprehensive filtering, searching, sorting, and pagination
    /// Inherits base pagination, search, and sort parameters from BasePagedQuery
    /// </summary>
    public class GetAllProductImageQuery : BasePagedQuery<ProductImageDto>
    {
        // Basic filters
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

        // Date range filters
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

    }
}
