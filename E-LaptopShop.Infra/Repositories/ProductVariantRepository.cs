using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class ProductVariantRepository : BaseRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(ApplicationDbContext context, ILogger<ProductVariantRepository> logger)
            : base(context, logger) { }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(
            int productId, CancellationToken ct = default)
        {
            return await _context.Set<ProductVariant>()
                .Include(v => v.AttributeValue)
                    .ThenInclude(av => av.Attribute)
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.CreatedAt)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<ProductVariant?> GetBySKUAsync(string sku, CancellationToken ct = default)
        {
            return await _context.Set<ProductVariant>()
                .Include(v => v.AttributeValue).ThenInclude(av => av.Attribute)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.SKU == sku, ct);
        }

        public async Task<bool> SKUExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default)
        {
            return await _context.Set<ProductVariant>()
                .AsNoTracking()
                .AnyAsync(v => v.SKU == sku && (excludeId == null || v.Id != excludeId), ct);
        }

        public async Task<ProductVariant> AddWithAttributesAsync(
            ProductVariant variant,
            IEnumerable<int> attributeValueIds,
            CancellationToken ct = default)
        {
            // Load AttributeValue entities to attach
            var ids = attributeValueIds.ToList();
            var attrValues = await _context.Set<ProductAttributeValue>()
                .Where(av => ids.Contains(av.Id))
                .ToListAsync(ct);

            variant.AttributeValue = attrValues;

            await _context.Set<ProductVariant>().AddAsync(variant, ct);
            await _context.SaveChangesAsync(ct);
            return variant;
        }

        public async Task DeleteByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var variants = await _context.Set<ProductVariant>()
                .Where(v => v.ProductId == productId)
                .ToListAsync(ct);

            _context.Set<ProductVariant>().RemoveRange(variants);
            await _context.SaveChangesAsync(ct);
        }
    }
}
