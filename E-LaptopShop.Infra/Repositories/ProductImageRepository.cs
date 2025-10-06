using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using E_LaptopShop.Application.Common.Exceptions;

namespace E_LaptopShop.Infra.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductImageRepository> _logger;

        public ProductImageRepository(ApplicationDbContext context, ILogger<ProductImageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ProductImage> AddImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            try
            {
                if (productImage == null)
                    Throw.IfNull(productImage, nameof(productImage));
                await _context.ProductImages.AddAsync(productImage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return productImage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding product image", ex);
            }
        }

        public async Task<ProductImage> ChangeActive(int id, bool isActive, CancellationToken cancellationToken)
        {
            try
            {
                if(id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
                var productImageStatus = _context.ProductImages.FirstOrDefault(x => x.Id == id);
                if (productImageStatus == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found.");
                productImageStatus.IsActive = isActive;
                await _context.SaveChangesAsync();
                return productImageStatus;

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error changing active status of product image", ex);
            }
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
                int countImage = await _context.ProductImages
                    .Where(x => x.ProductId == productId)
                    .CountAsync(cancellationToken);
                return countImage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error counting product images by product ID", ex);
            }
        }

        public async Task<int> DeleteImageAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than or equal to zero.");
                var productImage = _context.ProductImages.FirstOrDefault(x => x.Id == id);
                if (productImage == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found.");
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync(cancellationToken);
                return id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deleting product image", ex);
            }
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.ProductImages
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Error retrieving all product images", ex);
            }
        }

        public async Task<(IEnumerable<ProductImage> Items, int totalCount)> GetAllFilterAndPagination(
            int pageNumber,
            int pageSize, 
            ProductImageFilterParams filter,
            string? sortBy = null,
            bool isAscending = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Add maximum page size limit

                // Build base query with only necessary fields
                var query = _context.ProductImages
                    .AsNoTracking()
                    .Select(pi => new ProductImage
                    {
                        Id = pi.Id,
                        ProductId = pi.ProductId,
                        ImageUrl = pi.ImageUrl,
                        IsMain = pi.IsMain,
                        FileType = pi.FileType,
                        FileSize = pi.FileSize,
                        DisplayOrder = pi.DisplayOrder,
                        AltText = pi.AltText,
                        Title = pi.Title,
                        CreatedAt = pi.CreatedAt,
                        UploadedAt = pi.UploadedAt,
                        IsActive = pi.IsActive,
                        CreatedBy = pi.CreatedBy
                    });

                // Apply filters
                //query = ApplyFilter(query, filter);


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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered product images with pagination");
                throw new InvalidOperationException("Error retrieving filtered product images with pagination", ex);
            }
        }

        private IQueryable<ProductImage> ApplySorting(IQueryable<ProductImage> query, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Mặc định sắp xếp theo ngày tạo mới nhất
                return query.OrderByDescending(pi => pi.CreatedAt);
            }

            return sortBy.ToLower() switch
            {
                "id" => isAscending ? query.OrderBy(pi => pi.Id) : query.OrderByDescending(pi => pi.Id),
                "productid" => isAscending ? query.OrderBy(pi => pi.ProductId) : query.OrderByDescending(pi => pi.ProductId),
                "ismain" => isAscending ? query.OrderBy(pi => pi.IsMain) : query.OrderByDescending(pi => pi.IsMain),
                "filesize" => isAscending ? query.OrderBy(pi => pi.FileSize) : query.OrderByDescending(pi => pi.FileSize),
                "displayorder" => isAscending ? query.OrderBy(pi => pi.DisplayOrder) : query.OrderByDescending(pi => pi.DisplayOrder),
                "createdat" => isAscending ? query.OrderBy(pi => pi.CreatedAt) : query.OrderByDescending(pi => pi.CreatedAt),
                "uploadedat" => isAscending ? query.OrderBy(pi => pi.UploadedAt) : query.OrderByDescending(pi => pi.UploadedAt),
                "isactive" => isAscending ? query.OrderBy(pi => pi.IsActive) : query.OrderByDescending(pi => pi.IsActive),
                "title" => isAscending ? query.OrderBy(pi => pi.Title) : query.OrderByDescending(pi => pi.Title),
                "alttext" => isAscending ? query.OrderBy(pi => pi.AltText) : query.OrderByDescending(pi => pi.AltText),
                _ => query.OrderByDescending(pi => pi.CreatedAt) // Mặc định
            };
        }


        public async Task<ProductImage> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
               var productId = await _context.ProductImages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (productId == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found.");
                return productId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving product image by ID", ex);
            }
        }

        public async Task<IEnumerable<ProductImage>> GetFilteredAsync(
            ProductImageFilterParams filter,
            CancellationToken cancellationToken = default)
        {

            try
            {
                var query = _context.ProductImages.AsQueryable();
                //query = ApplyFilter(query, filter);
                return await query
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving filtered product images", ex);
            }
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            try
            {
                if(productId <=0)
                    throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
                var productImages = await _context.ProductImages
                    .AsNoTracking()
                    .Where(x => x.ProductId == productId)
                    .ToListAsync(cancellationToken);
                if (productImages == null)
                    throw new KeyNotFoundException($"No product images found for product ID {productId}.");
                return productImages;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving product images by product ID", ex);
            }
        }

        public async Task<ProductImage> GetMainImageByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            try
            {
                if(productId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
                var mainImage = await _context.ProductImages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsMain, cancellationToken);
                if (mainImage == null)
                    throw new KeyNotFoundException($"No main product image found for product ID {productId}.");
                return mainImage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving main product image by product ID", ex);
            }
        }

        public async Task<ProductImage> SetMainImageAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");

                var productImage = await _context.ProductImages
                    .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
                if (productImage == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found.");

                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var productImages = await _context.ProductImages
                        .Where(x => x.ProductId == productImage.ProductId)
                        .ToListAsync(cancellationToken);
                    foreach( var flagMainImage in productImages)
                        flagMainImage.IsMain = false;
                    
                    productImage.IsMain = true;

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return productImage;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new InvalidOperationException("Error setting main product image", ex);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error setting main product image", ex);
            }
        }

        public async Task<bool> UpdateDisplayOrderAsync(Dictionary<int, int> imageIdToOrderMap, CancellationToken cancellationToken)
        {
            try
            {
                //Validate input
                if (imageIdToOrderMap == null || imageIdToOrderMap.Count == 0)
                    throw new ArgumentNullException(nameof(imageIdToOrderMap), "Image ID to order map cannot be null or empty.");
                if (imageIdToOrderMap.Any(x => x.Key <= 0 || x.Value < 0))
                    throw new ArgumentOutOfRangeException(nameof(imageIdToOrderMap), "Image ID must be greater than zero and display order must be non-negative.");

                //Cặp Key - Value (ID - DisplayOrder => 1 - 0, 2 - 1, 3 - 2)
                //Giống HashTable
                var imageIDs = imageIdToOrderMap.Keys.ToList();
                var imagesValues = await _context.ProductImages
                    .Where(x => imageIDs.Contains(x.Id))
                    .ToListAsync(cancellationToken);
                if (imagesValues.Count != imageIDs.Count)
                    throw new KeyNotFoundException("Some product images not found.");
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach(var image in imagesValues)
                    {
                        image.DisplayOrder = imageIdToOrderMap[image.Id];
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new InvalidOperationException("Error updating display order of product images", ex);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating display order of product images", ex);
            }
        }

        public async Task<ProductImage> UpdateImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            try
            {

                if (productImage == null)
                    Throw.IfNull(productImage, nameof(productImage));
                if ( productImage.Id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(productImage.Id), "ID must be greater than zero.");
                var productImageToUpdate = await _context.ProductImages
                    .FirstOrDefaultAsync(x => x.Id == productImage.Id, cancellationToken);
                if (productImageToUpdate == null)
                    throw new KeyNotFoundException($"Product image with ID {productImage.Id} not found.");

                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    _context.Entry(productImageToUpdate).CurrentValues.SetValues(
                        typeof(Product).GetProperties()
                            .Where(p => p.CanWrite && p.GetValue(productImage) != null)
                            .ToDictionary(p => p.Name, p => p.GetValue(productImage))
                    );

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return productImageToUpdate;
                }
                catch (Exception ex) 
                { 
                    await transaction.RollbackAsync(cancellationToken);
                    throw new InvalidOperationException("Error updating product image", ex);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating product image", ex);
            }
        }

        public IQueryable<ProductImage> GetFilteredQueryable(ProductImageFilterParams filters)
        {
            try
            {
                var q = _context.ProductImages.AsQueryable()
                        .Include(q => q.Product)
                        .Include(q => q.SysFile);
                return q;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error building filtered queryable for product images", ex);
            }
        }
    }
}
