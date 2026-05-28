using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(
            ApplicationDbContext context,
            ILogger<CouponRepository> logger) : base(context, logger)
        {
        }

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            var normalized = code.Trim().ToUpper();
            return await GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code.ToUpper() == normalized, cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            var normalized = code.Trim().ToUpper();
            return await GetQueryable()
                .AsNoTracking()
                .AnyAsync(
                    c => c.Code.ToUpper() == normalized && (excludeId == null || c.Id != excludeId),
                    cancellationToken);
        }

        public async Task<int> IncrementUsedCountAsync(int couponId, CancellationToken cancellationToken = default)
        {
            var coupon = await _context.Set<Coupon>().FirstOrDefaultAsync(c => c.Id == couponId, cancellationToken)
                         ?? throw new KeyNotFoundException($"Coupon {couponId} not found");
            coupon.UsedCount += 1;
            await _context.SaveChangesAsync(cancellationToken);
            return coupon.UsedCount;
        }

        public async Task<int> CountUserUsageAsync(int couponId, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CouponUsage>()
                .AsNoTracking()
                .CountAsync(u => u.CouponId == couponId && u.UserId == userId, cancellationToken);
        }

        public async Task<CouponUsage> AddUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default)
        {
            await _context.Set<CouponUsage>().AddAsync(usage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return usage;
        }
    }
}
