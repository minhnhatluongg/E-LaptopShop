using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class BannerRepository : BaseRepository<Banner>, IBannerRepository
    {
        public BannerRepository(ApplicationDbContext context, ILogger<BannerRepository> logger)
            : base(context, logger) { }

        public async Task<IEnumerable<Banner>> GetActiveByPositionAsync(
            string position, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await GetQueryable()
                .Where(b => b.IsActive
                         && b.Position == position
                         && (b.StartsAt == null || b.StartsAt <= now)
                         && (b.EndsAt   == null || b.EndsAt   >= now))
                .OrderBy(b => b.DisplayOrder)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
