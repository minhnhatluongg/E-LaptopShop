using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IProductService : IBaseService<ProductDto, CreateProductRequestDto, UpdateProductRequestDto, ProductQueryParams>
    {
        Task<ProductDto> CreateProductAsync(CreateProductRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<ProductDto> UpdateProductAsync(UpdateProductRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        IQueryable<Product> GetProductsQueryable(
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null);

        Task<PagedResult<ProductDto>> GetAllProductsAsync(
            ProductQueryParams queryParams,
            CancellationToken cancellationToken = default);

        Task<bool> ValidateProductAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> IsProductInStockAsync(int id, int requiredQuantity = 1, CancellationToken cancellationToken = default);
        Task<decimal> CalculateDiscountedPriceAsync(int productId, decimal? discountPercentage = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 5, CancellationToken cancellationToken = default);
        Task UpdateProductStockAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    }
}
