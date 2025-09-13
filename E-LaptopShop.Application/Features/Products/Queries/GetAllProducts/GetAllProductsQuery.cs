using System.Collections.Generic;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Queries;

namespace E_LaptopShop.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : BasePagedQuery<ProductDto>
    {
        public int? CategoryId { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public bool? InStock { get; init; }
        public string? Brand { get; init; }
        public bool? IsActive { get; init; }
        public decimal? MinDiscount { get; init; }
        public decimal? MaxDiscount { get; init; }
    }
}
