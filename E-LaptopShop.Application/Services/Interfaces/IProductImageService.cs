using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IProductImageService : IBaseService<ProductImageDto, CreateProductImageRequestDto, UpdateProductImageRequestDto, ProductImageQueryParams>
    {
        #region Domain-Specific Business Operations
        Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(
            int productId,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);
        Task<ProductImageDto?> GetMainImageByProductIdAsync(
            int productId,
            CancellationToken cancellationToken = default);
        Task<ProductImageDto> SetMainImageAsync(
            int imageId,
            CancellationToken cancellationToken = default);
        Task<ProductImageDto> ChangeActiveStatusAsync(
            int imageId,
            bool isActive,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateDisplayOrderAsync(
            Dictionary<int, int> imageIdToOrderMap,
            CancellationToken cancellationToken = default);
        Task<int> CountImagesByProductIdAsync(
            int productId,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);
        Task<ProductImageDto> CreateWithSysFileAsync(
            int productId,
            int sysFileId,
            bool isMain = false,
            string? altText = null,
            string? title = null,
            string? createdBy = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region Legacy Methods (for backward compatibility)


        [Obsolete("Use GetByIdAsync instead")]
        Task<ProductImageDto> GetProductImageByIdAsync(int id, CancellationToken cancellationToken = default);


        [Obsolete("Use CreateAsync instead")]
        Task<ProductImageDto> CreateProductImageAsync(CreateProductImageRequestDto requestDto, CancellationToken cancellationToken = default);


        [Obsolete("Use UpdateAsync instead")]
        Task<ProductImageDto> UpdateProductImageAsync(UpdateProductImageRequestDto requestDto, CancellationToken cancellationToken = default);


        [Obsolete("Use DeleteAsync instead")]
        Task<int> DeleteProductImageAsync(int id, CancellationToken cancellationToken = default);

        #endregion

    }
}
