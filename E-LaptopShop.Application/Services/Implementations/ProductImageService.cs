using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace E_LaptopShop.Application.Services.Implementations
{
    /// <summary>
    /// ProductImageService implementation that extends BaseService for comprehensive CRUD and business operations
    /// Handles all ProductImage-related business logic, validation, and domain-specific operations
    /// </summary>
    public class ProductImageService : BaseService<ProductImage, ProductImageDto, CreateProductImageRequestDto, UpdateProductImageRequestDto, ProductImageQueryParams>, IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISysFileRepository _sysFileRepository;

        public ProductImageService(
            IProductImageRepository productImageRepository,
            IProductRepository productRepository,
            ISysFileRepository sysFileRepository,
            IMapper mapper,
            ILogger<ProductImageService> logger) : base(mapper, logger)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _sysFileRepository = sysFileRepository;
        }

        #region BaseService Implementation - Repository Bindings

        protected override async Task<ProductImage?> GetEntityByIdAsync(int id, CancellationToken ct)
        {
            return await _productImageRepository.GetByIdAsync(id, ct);
        }

        protected override async Task<ProductImage> CreateEntityAsync(ProductImage entity, CancellationToken ct)
        {
            return await _productImageRepository.AddImageAsync(entity, ct);
        }

        protected override async Task<ProductImage> UpdateEntityAsync(ProductImage entity, CancellationToken ct)
        {
            return await _productImageRepository.UpdateImageAsync(entity, ct);
        }

        protected override async Task<bool> DeleteEntityAsync(int id, CancellationToken ct)
        {
            var result = await _productImageRepository.DeleteImageAsync(id, ct);
            return result > 0;
        }

        protected override IQueryable<ProductImage> GetBaseQueryable(ProductImageQueryParams queryParams)
        {
            // Start with base queryable including necessary relationships
            return _productImageRepository.GetFilteredQueryable(new Domain.FilterParams.ProductImageFilterParams
            {
                Id = queryParams.Id,
                ProductId = queryParams.ProductId,
                IsMain = queryParams.IsMain,
                IsActive = queryParams.IsActive,
                ImageUrl = queryParams.ImageUrl,
                DisplayOrder = queryParams.DisplayOrder,
                Title = queryParams.Title,
                CreatedBy = queryParams.CreatedBy
            })
            .AsNoTracking();
        }

        protected override IQueryable<ProductImage> ApplyBusinessFilters(IQueryable<ProductImage> q, ProductImageQueryParams p)
        {
            // Apply domain-specific business filters
            if (p.ProductId.HasValue)
                q = q.Where(img => img.ProductId == p.ProductId.Value);

            if (p.IsMain.HasValue)
                q = q.Where(img => img.IsMain == p.IsMain.Value);

            if (p.IsActive.HasValue)
                q = q.Where(img => img.IsActive == p.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(p.FileType))
                q = q.Where(img => img.FileType.Contains(p.FileType));

            // File size range
            if (p.MinFileSize.HasValue)
                q = q.Where(img => img.FileSize >= p.MinFileSize.Value);

            if (p.MaxFileSize.HasValue)
                q = q.Where(img => img.FileSize <= p.MaxFileSize.Value);

            if (p.DisplayOrder.HasValue)
                q = q.Where(img => img.DisplayOrder == p.DisplayOrder.Value);

            // Date range filters
            if (p.CreatedAfter.HasValue)
                q = q.Where(img => img.CreatedAt >= p.CreatedAfter.Value);

            if (p.CreatedBefore.HasValue)
                q = q.Where(img => img.CreatedAt <= p.CreatedBefore.Value);

            if (p.UploadedAfter.HasValue)
                q = q.Where(img => img.UploadedAt >= p.UploadedAfter.Value);

            if (p.UploadedBefore.HasValue)
                q = q.Where(img => img.UploadedAt <= p.UploadedBefore.Value);

            if (!string.IsNullOrWhiteSpace(p.CreatedBy))
                q = q.Where(img => img.CreatedBy == p.CreatedBy);

            return q;
        }

        protected override IQueryable<ProductImage> ApplyDomainSearch(IQueryable<ProductImage> q, ProductImageQueryParams p)
        {
            if (string.IsNullOrWhiteSpace(p.Search))
                return q;

            var searchTerm = p.Search.Trim();

            // Try to parse as int for ID/DisplayOrder search
            int? searchInt = int.TryParse(searchTerm, out int parsedInt) ? parsedInt : null;

            return q.Where(img =>
                // Search in ImageUrl
                EF.Functions.Like(img.ImageUrl, $"%{searchTerm}%") ||
                // Search in AltText
                (img.AltText != null && EF.Functions.Like(img.AltText, $"%{searchTerm}%")) ||
                // Search in Title
                (img.Title != null && EF.Functions.Like(img.Title, $"%{searchTerm}%")) ||
                // Search in FileType
                EF.Functions.Like(img.FileType, $"%{searchTerm}%") ||
                // Search in CreatedBy
                (img.CreatedBy != null && EF.Functions.Like(img.CreatedBy, $"%{searchTerm}%")) ||
                // Search by DisplayOrder if search term is numeric
                (searchInt.HasValue && img.DisplayOrder == searchInt.Value) ||
                // Search in related Product name
                (img.Product != null && EF.Functions.Like(img.Product.Name, $"%{searchTerm}%"))
            );
        }

        protected override IQueryable<ProductImage> ApplyDomainSorting(IQueryable<ProductImage> q, ProductImageQueryParams p)
        {
            if (!string.IsNullOrWhiteSpace(p.SortBy))
            {
                return ApplySortingByMap(q, p.SortBy.ToLowerInvariant(), p.IsAscending);
            }

            return ApplyDefaultSorting(q);
        }

        #endregion

        #region BaseService Overrides - Business Logic Hooks

        protected override async Task ValidateCreateDto(CreateProductImageRequestDto dto, CancellationToken ct)
        {
            // Validate Product exists
            var product = await _productRepository.GetByIdAsync(dto.ProductId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

            // Validate SysFile exists if provided
            if (dto.SysFileId.HasValue)
            {
                var sysFile = await _sysFileRepository.GetByIdAsync(dto.SysFileId.Value, ct);
                if (sysFile == null)
                    throw new KeyNotFoundException($"SysFile with ID {dto.SysFileId} not found");
            }

            // Business rule: Only one main image per product
            if (dto.IsMain)
            {
                var existingMainImage = await _productImageRepository.GetMainImageByProductIdAsync(dto.ProductId, ct);
                if (existingMainImage != null)
                {
                    _logger.LogInformation("Product {ProductId} already has a main image. It will be updated to non-main.", dto.ProductId);
                }
            }
        }

        protected override async Task ValidateUpdateDto(int id, UpdateProductImageRequestDto dto, ProductImage existing, CancellationToken ct)
        {
            // Validate Product exists
            var product = await _productRepository.GetByIdAsync(dto.ProductId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

            // Validate SysFile exists if provided
            if (dto.SysFileId.HasValue)
            {
                var sysFile = await _sysFileRepository.GetByIdAsync(dto.SysFileId.Value, ct);
                if (sysFile == null)
                    throw new KeyNotFoundException($"SysFile with ID {dto.SysFileId} not found");
            }

            // Business rule: Only one main image per product
            if (dto.IsMain && !existing.IsMain)
            {
                var existingMainImage = await _productImageRepository.GetMainImageByProductIdAsync(dto.ProductId, ct);
                if (existingMainImage != null && existingMainImage.Id != id)
                {
                    _logger.LogInformation("Product {ProductId} already has a main image. It will be updated to non-main.", dto.ProductId);
                }
            }
        }

        protected override async Task ValidateDeleteRules(ProductImage entity, CancellationToken ct)
        {
            // Business rule: Warn if deleting the main image
            if (entity.IsMain)
            {
                _logger.LogWarning("Deleting main image {ImageId} for product {ProductId}. Consider setting another image as main first.", entity.Id, entity.ProductId);
            }

            // Could add more business rules here, e.g.:
            // - Don't allow deletion if it's the last image for a product
            // - Don't allow deletion if the product is in certain status
        }

        protected override async Task ApplyCreateBusinessRules(ProductImage entity, CreateProductImageRequestDto dto, CancellationToken ct)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UploadedAt = DateTime.UtcNow;
            entity.IsActive = true;

            if (dto.IsMain)
            {
                await UnsetOtherMainImagesAsync(dto.ProductId, ct);
            }

            if (entity.DisplayOrder == 0)
            {
                var maxOrder = await GetMaxDisplayOrderForProductAsync(dto.ProductId, ct);
                entity.DisplayOrder = maxOrder + 1;
            }
        }

        protected override async Task ApplyUpdateBusinessRules(ProductImage entity, UpdateProductImageRequestDto dto, CancellationToken ct)
        {
            // Handle main image logic
            if (dto.IsMain && !entity.IsMain)
            {
                await UnsetOtherMainImagesAsync(dto.ProductId, ct);
            }
        }

        protected override async Task ValidateQueryParams(ProductImageQueryParams p, CancellationToken ct)
        {
            await base.ValidateQueryParams(p, ct);

            // Custom validation and normalization
            p.ValidateAndNormalize();
            p.ValidateBusinessRules();
        }

        protected override bool HasSearchCriteria(ProductImageQueryParams p)
        {
            return p.HasSearchCriteria();
        }

        protected override IQueryable<ProductImage> ApplyDefaultSorting(IQueryable<ProductImage> q)
        {
            // Default sorting: Main images first, then by DisplayOrder, then by CreatedAt descending
            return q.OrderByDescending(img => img.IsMain)
                    .ThenBy(img => img.DisplayOrder)
                    .ThenByDescending(img => img.CreatedAt);
        }

        protected override IReadOnlyDictionary<string, Expression<Func<ProductImage, object>>> SortMap =>
            new Dictionary<string, Expression<Func<ProductImage, object>>>
            {
                ["id"] = img => img.Id,
                ["productid"] = img => img.ProductId,
                ["imageurl"] = img => img.ImageUrl,
                ["ismain"] = img => img.IsMain,
                ["filetype"] = img => img.FileType,
                ["filesize"] = img => img.FileSize,
                ["displayorder"] = img => img.DisplayOrder,
                ["alttext"] = img => img.AltText ?? "",
                ["title"] = img => img.Title ?? "",
                ["createdat"] = img => img.CreatedAt,
                ["uploadedat"] = img => img.UploadedAt,
                ["isactive"] = img => img.IsActive,
                ["createdby"] = img => img.CreatedBy ?? "",
                ["productname"] = img => img.Product != null ? img.Product.Name : ""
            };

        #endregion

        #region Domain-Specific Business Operations

        public async Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(int productId, bool onlyActive = true, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting images for product {ProductId}, onlyActive: {OnlyActive}", productId, onlyActive);

            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than zero", nameof(productId));

            var images = await _productImageRepository.GetImagesByProductIdAsync(productId, cancellationToken);
            
            if (onlyActive)
                images = images.Where(img => img.IsActive);

            var result = images.OrderByDescending(img => img.IsMain)
                              .ThenBy(img => img.DisplayOrder)
                              .ThenByDescending(img => img.CreatedAt);

            return _mapper.Map<IEnumerable<ProductImageDto>>(result);
        }

        public async Task<ProductImageDto?> GetMainImageByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting main image for product {ProductId}", productId);

            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than zero", nameof(productId));

            try
            {
                var mainImage = await _productImageRepository.GetMainImageByProductIdAsync(productId, cancellationToken);
                return _mapper.Map<ProductImageDto>(mainImage);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogInformation("No main image found for product {ProductId}", productId);
                return null;
            }
        }

        public async Task<ProductImageDto> SetMainImageAsync(int imageId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Setting image {ImageId} as main image", imageId);

            if (imageId <= 0)
                throw new ArgumentException("Image ID must be greater than zero", nameof(imageId));

            var updatedImage = await _productImageRepository.SetMainImageAsync(imageId, cancellationToken);
            return _mapper.Map<ProductImageDto>(updatedImage);
        }

        public async Task<ProductImageDto> ChangeActiveStatusAsync(int imageId, bool isActive, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Changing active status of image {ImageId} to {IsActive}", imageId, isActive);

            if (imageId <= 0)
                throw new ArgumentException("Image ID must be greater than zero", nameof(imageId));

            var updatedImage = await _productImageRepository.ChangeActive(imageId, isActive, cancellationToken);
            return _mapper.Map<ProductImageDto>(updatedImage);
        }

        public async Task<bool> UpdateDisplayOrderAsync(
                    Dictionary<int, int>? imageIdToOrderMap,
                    CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating display order for {count} images", imageIdToOrderMap?.Count ?? 0);

            ArgumentNullException.ThrowIfNull(imageIdToOrderMap);
            if (imageIdToOrderMap.Count == 0)
                Throw.BadRequest("Image ID to order map cannot be empty");

            return await _productImageRepository.UpdateDisplayOrderAsync(imageIdToOrderMap, cancellationToken);
        }

        public async Task<int> CountImagesByProductIdAsync(int productId, bool onlyActive = true, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Counting images for product {ProductId}, onlyActive: {OnlyActive}", productId, onlyActive);

            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than zero", nameof(productId));

            var totalCount = await _productImageRepository.CountByProductIdAsync(productId, cancellationToken);

            if (!onlyActive)
                return totalCount;

            // If only active requested, we need to filter
            var images = await _productImageRepository.GetImagesByProductIdAsync(productId, cancellationToken);
            return images.Count(img => img.IsActive);
        }

        public async Task<ProductImageDto> CreateWithSysFileAsync(int productId, int sysFileId, bool isMain = false, string? altText = null, string? title = null, string? createdBy = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating product image with SysFile - ProductId: {ProductId}, SysFileId: {SysFileId}", productId, sysFileId);

            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than zero", nameof(productId));

            if (sysFileId <= 0)
                throw new ArgumentException("SysFile ID must be greater than zero", nameof(sysFileId));

            // Get SysFile information
            var sysFile = await _sysFileRepository.GetByIdAsync(sysFileId, cancellationToken);
            if (sysFile == null)
                throw new KeyNotFoundException($"SysFile with ID {sysFileId} not found");

            // Create DTO with SysFile information
            var createDto = new CreateProductImageRequestDto
            {
                ProductId = productId,
                SysFileId = sysFileId,
                ImageUrl = sysFile.FileUrl,
                FileType = sysFile.FileType,
                FileSize = sysFile.FileSize,
                IsMain = isMain,
                AltText = altText,
                Title = title,
                CreatedBy = createdBy
            };

            return await CreateAsync(createDto, cancellationToken);
        }
        #endregion

        #region Legacy Methods (for backward compatibility)

        [Obsolete("Use GetByIdAsync instead")]
        public async Task<ProductImageDto> GetProductImageByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await GetByIdAsync(id, cancellationToken);
            return result ?? throw new KeyNotFoundException($"ProductImage with ID {id} not found");
        }

        [Obsolete("Use CreateAsync instead")]
        public async Task<ProductImageDto> CreateProductImageAsync(CreateProductImageRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(requestDto, cancellationToken);
        }

        [Obsolete("Use UpdateAsync instead")]
        public async Task<ProductImageDto> UpdateProductImageAsync(UpdateProductImageRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(requestDto.Id, requestDto, cancellationToken);
        }

        [Obsolete("Use DeleteAsync instead")]
        public async Task<int> DeleteProductImageAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await DeleteAsync(id, cancellationToken);
            return result ? id : throw new InvalidOperationException($"Failed to delete product image with ID {id}");
        }

        #endregion

        #region Private Helper Methods

        private async Task UnsetOtherMainImagesAsync(int productId, CancellationToken ct)
        {
            try
            {
                var existingMainImage = await _productImageRepository.GetMainImageByProductIdAsync(productId, ct);
                if (existingMainImage != null)
                {
                    existingMainImage.IsMain = false;
                    await _productImageRepository.UpdateImageAsync(existingMainImage, ct);
                    _logger.LogInformation("Unset main image {ImageId} for product {ProductId}", existingMainImage.Id, productId);
                }
            }
            catch (KeyNotFoundException)
            {
                // No existing main image, which is fine
            }
        }

        private async Task<int> GetMaxDisplayOrderForProductAsync(int productId, CancellationToken ct)
        {
            var images = await _productImageRepository.GetImagesByProductIdAsync(productId, ct);
            return images.Any() ? images.Max(img => img.DisplayOrder) : 0;
        }

        #endregion
    }
}
