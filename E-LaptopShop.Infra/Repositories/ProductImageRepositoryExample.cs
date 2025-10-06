using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    /// <summary>
    /// EXAMPLE: ProductImageRepository refactored using BaseRepository pattern
    /// This demonstrates how to implement repositories with minimal boilerplate code
    /// while maintaining all existing functionality and adding new capabilities
    /// </summary>
    public class ProductImageRepositoryExample : BaseRepository<ProductImage>, IProductImageRepository
    {
        // ApplicationDbContext cast for domain-specific operations
        private readonly ApplicationDbContext _appContext;

        public ProductImageRepositoryExample(ApplicationDbContext context, ILogger<ProductImageRepositoryExample> logger) 
            : base(context, logger)
        {
            _appContext = context;
        }

        #region BaseRepository Inherited Methods (Available automatically)
        
        // ✅ CRUD operations inherited from BaseRepository:
        // - Task<ProductImage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        // - Task<IEnumerable<ProductImage>> GetAllAsync(CancellationToken cancellationToken = default)
        // - Task<ProductImage> AddAsync(ProductImage entity, CancellationToken cancellationToken = default)
        // - Task<ProductImage> UpdateAsync(ProductImage entity, CancellationToken cancellationToken = default)
        // - Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        // - Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)

        // ✅ Query operations inherited from BaseRepository:
        // - IQueryable<ProductImage> GetQueryable()
        // - Task<IEnumerable<ProductImage>> GetWhereAsync(Expression<Func<ProductImage, bool>> predicate, CancellationToken cancellationToken = default)
        // - Task<ProductImage?> GetSingleWhereAsync(Expression<Func<ProductImage, bool>> predicate, CancellationToken cancellationToken = default)
        // - Task<ProductImage?> GetFirstWhereAsync(Expression<Func<ProductImage, bool>> predicate, CancellationToken cancellationToken = default)
        // - Task<int> CountAsync(Expression<Func<ProductImage, bool>>? predicate = null, CancellationToken cancellationToken = default)
        // - Task<bool> AnyAsync(Expression<Func<ProductImage, bool>> predicate, CancellationToken cancellationToken = default)

        // ✅ Batch operations inherited from BaseRepository:
        // - Task<IEnumerable<ProductImage>> AddRangeAsync(IEnumerable<ProductImage> entities, CancellationToken cancellationToken = default)
        // - Task<IEnumerable<ProductImage>> UpdateRangeAsync(IEnumerable<ProductImage> entities, CancellationToken cancellationToken = default)
        // - Task<int> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        // - Task<int> DeleteWhereAsync(Expression<Func<ProductImage, bool>> predicate, CancellationToken cancellationToken = default)

        // ✅ Transaction support inherited from BaseRepository:
        // - Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        // - Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)

        #endregion

        #region IProductImageRepository Implementation (Legacy methods using BaseRepository)

        /// <summary>
        /// Legacy method - delegates to BaseRepository.AddAsync
        /// </summary>
        public async Task<ProductImage> AddImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            return await AddAsync(productImage, cancellationToken);
        }

        /// <summary>
        /// Legacy method - delegates to BaseRepository.UpdateAsync
        /// </summary>
        public async Task<ProductImage> UpdateImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            return await UpdateAsync(productImage, cancellationToken);
        }

        /// <summary>
        /// Legacy method - converts BaseRepository.DeleteAsync return type
        /// </summary>
        public async Task<int> DeleteImageAsync(int id, CancellationToken cancellationToken)
        {
            var result = await DeleteAsync(id, cancellationToken);
            return result ? id : throw new InvalidOperationException($"Failed to delete product image with ID {id}");
        }

        #endregion

        #region Domain-Specific Operations (ProductImage business logic)

        /// <summary>
        /// Gets all images for a specific product
        /// Uses BaseRepository.GetWhereAsync for efficient filtering
        /// </summary>
        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero");

            return await GetWhereAsync(img => img.ProductId == productId, cancellationToken);
        }

        /// <summary>
        /// Gets the main image for a specific product
        /// Uses BaseRepository.GetFirstWhereAsync for efficient single result
        /// </summary>
        public async Task<ProductImage> GetMainImageByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero");

            var mainImage = await GetFirstWhereAsync(img => img.ProductId == productId && img.IsMain, cancellationToken);
            return mainImage ?? throw new KeyNotFoundException($"No main product image found for product ID {productId}");
        }

        /// <summary>
        /// Sets an image as the main image for its product
        /// Uses BaseRepository transaction support for atomic operations
        /// </summary>
        public async Task<ProductImage> SetMainImageAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");

            using var transaction = await BeginTransactionAsync(cancellationToken);
            try
            {
                // Get the image to set as main
                var productImage = await GetByIdAsync(id, cancellationToken);
                if (productImage == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found");

                // Unset other main images for the same product
                var otherMainImages = await GetWhereAsync(
                    img => img.ProductId == productImage.ProductId && img.IsMain && img.Id != id,
                    cancellationToken);

                foreach (var otherImage in otherMainImages)
                {
                    otherImage.IsMain = false;
                    await UpdateAsync(otherImage, cancellationToken);
                }

                // Set this image as main
                productImage.IsMain = true;
                var updatedImage = await UpdateAsync(productImage, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return updatedImage;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Changes the active status of a product image
        /// Uses BaseRepository.UpdateAsync with business logic
        /// </summary>
        public async Task<ProductImage> ChangeActive(int id, bool isActive, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");

            var productImage = await GetByIdAsync(id, cancellationToken);
            if (productImage == null)
                throw new KeyNotFoundException($"Product image with ID {id} not found");

            productImage.IsActive = isActive;
            return await UpdateAsync(productImage, cancellationToken);
        }

        /// <summary>
        /// Counts images for a specific product
        /// Uses BaseRepository.CountAsync with predicate
        /// </summary>
        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero");

            return await CountAsync(img => img.ProductId == productId, cancellationToken);
        }

        /// <summary>
        /// Updates display order for multiple images
        /// Uses BaseRepository transaction and batch operations
        /// </summary>
        public async Task<bool> UpdateDisplayOrderAsync(Dictionary<int, int> imageIdToOrderMap, CancellationToken cancellationToken)
        {
            if (imageIdToOrderMap == null || imageIdToOrderMap.Count == 0)
                throw new ArgumentException("Image ID to order map cannot be null or empty", nameof(imageIdToOrderMap));

            if (imageIdToOrderMap.Any(x => x.Key <= 0 || x.Value < 0))
                throw new ArgumentOutOfRangeException(nameof(imageIdToOrderMap), "Image ID must be greater than zero and display order must be non-negative");

            using var transaction = await BeginTransactionAsync(cancellationToken);
            try
            {
                var imageIds = imageIdToOrderMap.Keys.ToList();
                var images = await GetWhereAsync(img => imageIds.Contains(img.Id), cancellationToken);
                var imageList = images.ToList();

                if (imageList.Count != imageIds.Count)
                    throw new KeyNotFoundException("Some product images not found");

                foreach (var image in imageList)
                {
                    image.DisplayOrder = imageIdToOrderMap[image.Id];
                }

                await UpdateRangeAsync(imageList, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Gets filtered entities based on filter parameters
        /// Uses BaseRepository.GetWhereAsync with complex filtering
        /// </summary>
        public async Task<IEnumerable<ProductImage>> GetFilteredAsync(
            ProductImageFilterParams filter,
            CancellationToken cancellationToken = default)
        {
            if (filter == null)
                return await GetAllAsync(cancellationToken);

            // Build predicate based on filter parameters
            return await GetWhereAsync(img => 
                (!filter.Id.HasValue || img.Id == filter.Id.Value) &&
                (!filter.ProductId.HasValue || img.ProductId == filter.ProductId.Value) &&
                (!filter.IsMain.HasValue || img.IsMain == filter.IsMain.Value) &&
                (!filter.IsActive.HasValue || img.IsActive == filter.IsActive.Value) &&
                (string.IsNullOrEmpty(filter.ImageUrl) || img.ImageUrl.Contains(filter.ImageUrl)) &&
                (!filter.DisplayOrder.HasValue || img.DisplayOrder == filter.DisplayOrder.Value) &&
                (string.IsNullOrEmpty(filter.Title) || (img.Title != null && img.Title.Contains(filter.Title))) &&
                (string.IsNullOrEmpty(filter.CreatedBy) || (img.CreatedBy != null && img.CreatedBy.Contains(filter.CreatedBy))),
                cancellationToken);
        }

        /// <summary>
        /// Gets queryable with filtering applied
        /// Uses BaseRepository.GetQueryable() with custom filtering and includes
        /// </summary>
        public IQueryable<ProductImage> GetFilteredQueryable(ProductImageFilterParams filters)
        {
            IQueryable<ProductImage> query = GetQueryable();
            query = GetQueryable()
                .Include(img => img.Product)
                .Include(img => img.SysFile);
            if (filters == null)
                return query;

            if (filters.Id.HasValue)
                query = query.Where(img => img.Id == filters.Id.Value);

            if (filters.ProductId.HasValue)
                query = query.Where(img => img.ProductId == filters.ProductId.Value);

            if (filters.IsMain.HasValue)
                query = query.Where(img => img.IsMain == filters.IsMain.Value);

            if (filters.IsActive.HasValue)
                query = query.Where(img => img.IsActive == filters.IsActive.Value);

            if (!string.IsNullOrEmpty(filters.ImageUrl))
                query = query.Where(img => img.ImageUrl.Contains(filters.ImageUrl));

            if (filters.DisplayOrder.HasValue)
                query = query.Where(img => img.DisplayOrder == filters.DisplayOrder.Value);

            if (!string.IsNullOrEmpty(filters.Title))
                query = query.Where(img => img.Title != null && img.Title.Contains(filters.Title));

            if (!string.IsNullOrEmpty(filters.CreatedBy))
                query = query.Where(img => img.CreatedBy != null && img.CreatedBy.Contains(filters.CreatedBy));

            return query;
        }

        /// <summary>
        /// Legacy pagination method - can be implemented using BaseRepository methods
        /// </summary>
        public async Task<(IEnumerable<ProductImage> Items, int totalCount)> GetAllFilterAndPagination(
            int pageNumber,
            int pageSize,
            ProductImageFilterParams filter,
            string? sortBy = null,
            bool isAscending = true,
            CancellationToken cancellationToken = default)
        {
            // Validate pagination parameters
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            // Get filtered queryable
            var query = GetFilteredQueryable(filter).AsNoTracking();

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, sortBy, isAscending);

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        #endregion

        #region BaseRepository Overrides (Include relationships)

        /// <summary>
        /// Override to include related data when getting by ID
        /// </summary>
        public override async Task<ProductImage?> GetByIdWithIncludesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .Include(img => img.Product)
                .Include(img => img.SysFile)
                .AsNoTracking()
                .FirstOrDefaultAsync(img => img.Id == id, cancellationToken);
        }

        /// <summary>
        /// Override to include related data when getting all
        /// </summary>
        public override async Task<IEnumerable<ProductImage>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .Include(img => img.Product)
                .Include(img => img.SysFile)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Override to apply business rules before adding
        /// </summary>
        protected override async Task ValidateBeforeAdd(ProductImage entity, CancellationToken cancellationToken)
        {
            await base.ValidateBeforeAdd(entity, cancellationToken);

            // Validate product exists
            if (!await _appContext.Products.AnyAsync(p => p.Id == entity.ProductId, cancellationToken))
                throw new InvalidOperationException($"Product with ID {entity.ProductId} does not exist");

            // Validate SysFile exists if provided
            if (entity.SysFileId.HasValue)
            {
                if (!await _appContext.SysFiles.AnyAsync(f => f.Id == entity.SysFileId.Value, cancellationToken))
                    throw new InvalidOperationException($"SysFile with ID {entity.SysFileId} does not exist");
            }
        }

        /// <summary>
        /// Override to apply business rules before updating
        /// </summary>
        protected override async Task ValidateBeforeUpdate(ProductImage entity, CancellationToken cancellationToken)
        {
            await base.ValidateBeforeUpdate(entity, cancellationToken);

            // Validate product exists
            if (!await _appContext.Products.AnyAsync(p => p.Id == entity.ProductId, cancellationToken))
                throw new InvalidOperationException($"Product with ID {entity.ProductId} does not exist");
        }

        /// <summary>
        /// Override to apply business rules before deleting
        /// </summary>
        protected override async Task ValidateBeforeDelete(ProductImage entity, CancellationToken cancellationToken)
        {
            await base.ValidateBeforeDelete(entity, cancellationToken);

            // Warn if deleting main image
            if (entity.IsMain)
            {
                _logger.LogWarning("Deleting main image {ImageId} for product {ProductId}", entity.Id, entity.ProductId);
            }
        }

        /// <summary>
        /// Override to apply business rules when adding
        /// </summary>
        protected override async Task ApplyAddBusinessRules(ProductImage entity, CancellationToken cancellationToken)
        {
            await base.ApplyAddBusinessRules(entity, cancellationToken);

            // Set timestamps
            entity.CreatedAt = DateTime.UtcNow;
            entity.UploadedAt = DateTime.UtcNow;
            entity.IsActive = true;

            // Auto-set display order if not provided
            if (entity.DisplayOrder == 0)
            {
                var maxOrder = await GetQueryable()
                    .Where(img => img.ProductId == entity.ProductId)
                    .MaxAsync(img => (int?)img.DisplayOrder, cancellationToken) ?? 0;
                entity.DisplayOrder = maxOrder + 1;
            }
        }

        #endregion

        #region Private Helper Methods

        private IQueryable<ProductImage> ApplySorting(IQueryable<ProductImage> query, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default sorting: Main images first, then by DisplayOrder, then by CreatedAt descending
                return query.OrderByDescending(img => img.IsMain)
                           .ThenBy(img => img.DisplayOrder)
                           .ThenByDescending(img => img.CreatedAt);
            }

            return sortBy.ToLower() switch
            {
                "id" => isAscending ? query.OrderBy(img => img.Id) : query.OrderByDescending(img => img.Id),
                "productid" => isAscending ? query.OrderBy(img => img.ProductId) : query.OrderByDescending(img => img.ProductId),
                "ismain" => isAscending ? query.OrderBy(img => img.IsMain) : query.OrderByDescending(img => img.IsMain),
                "filesize" => isAscending ? query.OrderBy(img => img.FileSize) : query.OrderByDescending(img => img.FileSize),
                "displayorder" => isAscending ? query.OrderBy(img => img.DisplayOrder) : query.OrderByDescending(img => img.DisplayOrder),
                "createdat" => isAscending ? query.OrderBy(img => img.CreatedAt) : query.OrderByDescending(img => img.CreatedAt),
                "uploadedat" => isAscending ? query.OrderBy(img => img.UploadedAt) : query.OrderByDescending(img => img.UploadedAt),
                "isactive" => isAscending ? query.OrderBy(img => img.IsActive) : query.OrderByDescending(img => img.IsActive),
                _ => query.OrderByDescending(img => img.CreatedAt) // Default fallback
            };
        }

        #endregion
    }
}
