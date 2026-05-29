using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class ProductAttributeRepository : BaseRepository<ProductAttribute>, IProductAttributeRepository
    {
        public ProductAttributeRepository(ApplicationDbContext context, ILogger<ProductAttributeRepository> logger)
            : base(context, logger) { }

        public async Task<IEnumerable<ProductAttribute>> GetAllActiveWithValuesAsync(CancellationToken ct = default)
        {
            return await _context.Set<ProductAttribute>()
                .Include(a => a.ProductAttributeValues.OrderBy(v => v.DisplayOrder))
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<ProductAttributeValue?> GetValueByIdAsync(int valueId, CancellationToken ct = default)
        {
            return await _context.Set<ProductAttributeValue>()
                .Include(v => v.Attribute)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == valueId, ct);
        }

        public async Task<ProductAttributeValue> AddValueAsync(
            ProductAttributeValue value, CancellationToken ct = default)
        {
            await _context.Set<ProductAttributeValue>().AddAsync(value, ct);
            await _context.SaveChangesAsync(ct);
            return value;
        }

        public async Task<bool> DeleteValueAsync(int valueId, CancellationToken ct = default)
        {
            var value = await _context.Set<ProductAttributeValue>().FindAsync(new object[] { valueId }, ct);
            if (value == null) return false;
            _context.Set<ProductAttributeValue>().Remove(value);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
