using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class ProductCommentRepository : IProductCommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductCommentRepository> _logger;

        public ProductCommentRepository(ApplicationDbContext context, ILogger<ProductCommentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductComment>> GetByProductIdAsync(int productId, CancellationToken ct = default)
        {
            return await _context.ProductComments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.ProductId == productId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<ProductComment?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.ProductComments.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<ProductComment> AddAsync(ProductComment comment, CancellationToken ct = default)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            comment.CreatedAt = DateTime.UtcNow;
            await _context.ProductComments.AddAsync(comment, ct);
            await _context.SaveChangesAsync(ct);
            return comment;
        }

        public async Task<int> SoftDeleteAsync(int id, CancellationToken ct = default)
        {
            var c = await _context.ProductComments.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (c == null) throw new KeyNotFoundException($"Comment {id} not found");
            c.IsDeleted = true;
            c.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return id;
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken ct = default)
            => await _context.ProductComments.CountAsync(c => c.ProductId == productId && !c.IsDeleted, ct);
    }
}
