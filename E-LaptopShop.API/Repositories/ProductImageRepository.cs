using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.API.Repositories
{
    public class ProductImageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductImageRepository> _logger;

        public ProductImageRepository(ApplicationDbContext context, ILogger<ProductImageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            try
            {
                if (productId <= 0)
                    throw new ArgumentException("Product ID must be greater than zero", nameof(productId));

                var productImages = await _context.ProductImages
                    .AsNoTracking()
                    .Where(x => x.ProductId == productId)
                    .ToListAsync(cancellationToken);

                if (!productImages.Any())
                    _logger.LogWarning($"No images found for product with ID {productId}");

                return productImages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product images for product ID {productId}");
                throw new InvalidOperationException($"Error retrieving product images for product ID {productId}", ex);
            }
        }

        public async Task<ProductImage> SetMainImageAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Image ID must be greater than zero", nameof(id));

                // Get the image to be set as main
                var productImage = await _context.ProductImages
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (productImage == null)
                    throw new KeyNotFoundException($"Product image with ID {id} not found.");

                // Start transaction
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Reset IsMain flag for all images of the same product
                    var productImages = await _context.ProductImages
                        .Where(x => x.ProductId == productImage.ProductId)
                        .ToListAsync(cancellationToken);

                    foreach (var image in productImages)
                    {
                        image.IsMain = false;
                    }

                    // Set the selected image as main
                    productImage.IsMain = true;

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation($"Successfully set image {id} as main image for product {productImage.ProductId}");
                    return productImage;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, $"Error occurred while setting main image {id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting main product image for ID {id}");
                throw new InvalidOperationException($"Error setting main product image for ID {id}", ex);
            }
        }

        public async Task<bool> UpdateDisplayOrderAsync(Dictionary<int, int> imageIdToOrderMap, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (imageIdToOrderMap == null || !imageIdToOrderMap.Any())
                    throw new ArgumentException("Image ID to order map cannot be null or empty", nameof(imageIdToOrderMap));

                if (imageIdToOrderMap.Values.Any(order => order < 0))
                    throw new ArgumentException("Display order cannot be negative", nameof(imageIdToOrderMap));

                // Check for duplicate order values
                if (imageIdToOrderMap.Values.Distinct().Count() != imageIdToOrderMap.Count)
                    throw new ArgumentException("Duplicate display order values are not allowed", nameof(imageIdToOrderMap));

                // Get all images that need to be updated
                var imageIds = imageIdToOrderMap.Keys.ToList();
                var images = await _context.ProductImages
                    .Where(x => imageIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                // Validate that all requested images exist
                if (images.Count != imageIds.Count)
                {
                    var missingIds = imageIds.Except(images.Select(x => x.Id));
                    throw new KeyNotFoundException($"Some images were not found: {string.Join(", ", missingIds)}");
                }

                // Start transaction
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Update display order for each image
                    foreach (var image in images)
                    {
                        image.DisplayOrder = imageIdToOrderMap[image.Id];
                    }

                    // Save changes
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation($"Successfully updated display order for {images.Count} images");
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error occurred while updating display order");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating display order of product images");
                throw new InvalidOperationException("Error updating display order of product images", ex);
            }
        }

        public async Task<ProductImage> UpdateImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (productImage == null)
                    throw new ArgumentNullException(nameof(productImage));

                if (productImage.Id <= 0)
                    throw new ArgumentException("Product image ID must be greater than zero", nameof(productImage));

                // Get existing image
                var productImageToUpdate = await _context.ProductImages
                    .FirstOrDefaultAsync(x => x.Id == productImage.Id, cancellationToken);

                if (productImageToUpdate == null)
                    throw new KeyNotFoundException($"Product image with ID {productImage.Id} not found.");

                // Start transaction
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Update properties using reflection
                    var properties = typeof(ProductImage).GetProperties()
                        .Where(p => p.CanWrite && p.Name != nameof(ProductImage.Id)); // Exclude Id property

                    foreach (var property in properties)
                    {
                        var newValue = property.GetValue(productImage);
                        if (newValue != null) // Only update if new value is not null
                        {
                            property.SetValue(productImageToUpdate, newValue);
                        }
                    }

                    // Save changes
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation($"Successfully updated product image with ID {productImage.Id}");
                    return productImageToUpdate;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, $"Error occurred while updating product image {productImage.Id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product image {productImage?.Id}");
                throw new InvalidOperationException($"Error updating product image {productImage?.Id}", ex);
            }
        }
    }
} 