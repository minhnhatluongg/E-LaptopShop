using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepositoy
    {

        public BrandRepository(
            ApplicationDbContext context,
            ILogger<BrandRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Brand>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            return await GetQueryable()
                .Where(b => b.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
