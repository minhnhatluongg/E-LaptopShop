﻿using E_LaptopShop.Application.Common.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetFilterdPagedCategoriesQuery
{
    public class GetFilteredPagedCategoriesQuery : IRequest<PagedResult<CategoryDto>>
    {
        //Dùng để phân trang
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        //Lọc
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

        //Sắp xếp
        public string? SortBy { get; init; }
        public bool IsAscending { get; init; } = true;

        // Tìm kiếm
        public string? SearchTerm { get; init; }
        public string[]? SearchFields { get; init; }
    }
}
