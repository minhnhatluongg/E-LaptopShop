using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetAllFilteredAndPagination
{
    public class GetAllProductImageQuery : BasePagedQuery<ProductImageDto>
    {
        // Filtering
        public int? Id { get; init; }
        public int? ProductId { get; init; }
        public string? ImageUrl { get; init; }
        public bool? IsMain { get; init; }
        public string? FileType { get; init; }
        public long? FileSize { get; init; }
        public int? DisplayOrder { get; init; }
        public string? AltText { get; init; }
        public string? Title { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UploadedAt { get; init; }
        public bool? IsActive { get; init; }
        public string? CreatedBy { get; init; }
    }
}
