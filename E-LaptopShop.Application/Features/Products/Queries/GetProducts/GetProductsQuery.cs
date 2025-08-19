using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQuery : BasePagedQuery<ProductDto>
    {
        // Specific filters
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public bool? IsActive { get; set; }
    }
}

