using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : BasePagedQuery<ProductDto>
    {
        [FromQuery(Name = "categoryId")]
        public int? CategoryId { get; init; }

        [FromQuery(Name = "minPrice")]
        public decimal? MinPrice { get; init; }

        [FromQuery(Name = "maxPrice")]
        public decimal? MaxPrice { get; init; }

        [FromQuery(Name = "inStock")]
        public bool? InStock { get; init; }

        [FromQuery(Name = "brandId")]
        public int? BrandId { get; init; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; init; }

        [FromQuery(Name = "minDiscount")]
        public decimal? MinDiscount { get; init; }

        [FromQuery(Name = "maxDiscount")]
        public decimal? MaxDiscount { get; init; }

    }
}
