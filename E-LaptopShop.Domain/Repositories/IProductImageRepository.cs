using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IProductImageRepository : IBaseRepository<ProductImage>
    {
        Task<ProductImage> AddImageAsync(ProductImage productImage, CancellationToken cancellationToken);
        Task<ProductImage> UpdateImageAsync(ProductImage productImage, CancellationToken cancellationToken);
        Task<int> DeleteImageAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<ProductImage> ChangeActive (int id, bool isActive, CancellationToken cancellationToken);
        Task<ProductImage> SetMainImageAsync(int id, CancellationToken cancellationToken);
        Task<ProductImage> GetMainImageByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<bool> UpdateDisplayOrderAsync(Dictionary<int, int> imageIdToOrderMap, CancellationToken cancellationToken);
        Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken);

        Task<IEnumerable<ProductImage>> GetFilteredAsync(
            ProductImageFilterParams filter,
            CancellationToken cancellationToken = default
        );

        IQueryable<ProductImage> GetFilteredQueryable(ProductImageFilterParams filters);

        Task<(IEnumerable<ProductImage> Items, int totalCount)> GetAllFilterAndPagination(
            int pageNumber,
            int pageSize,
            ProductImageFilterParams filter,
            string? sortBy = null,
            bool isAscending = true,
            CancellationToken cancellationToken = default
        );
    }
}
