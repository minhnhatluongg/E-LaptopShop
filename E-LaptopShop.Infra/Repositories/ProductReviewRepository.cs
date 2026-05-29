using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductReviewRepository> _logger;

        // Order status được tính là "đã mua thành công"
        private static readonly string[] PurchasedStatuses = new[]
        {
            OrderStatus.Delivered.ToString(),
            OrderStatus.Completed.ToString()
        };

        public ProductReviewRepository(ApplicationDbContext context, ILogger<ProductReviewRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, CancellationToken ct = default)
        {
            return await _context.ProductReviews
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<ProductReview?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.ProductReviews.FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<ProductReview> AddAsync(ProductReview review, CancellationToken ct = default)
        {
            if (review == null) throw new ArgumentNullException(nameof(review));
            review.CreatedAt = DateTime.UtcNow;
            await _context.ProductReviews.AddAsync(review, ct);
            await _context.SaveChangesAsync(ct);
            return review;
        }

        public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
        {
            var r = await _context.ProductReviews.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (r == null) throw new KeyNotFoundException($"Review {id} not found");
            _context.ProductReviews.Remove(r);
            await _context.SaveChangesAsync(ct);
            return id;
        }

        public async Task<int> CountByProductIdAsync(int productId, CancellationToken ct = default)
            => await _context.ProductReviews.CountAsync(r => r.ProductId == productId && r.Rating != null, ct);

        public async Task<double> AverageRatingByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var ratings = await _context.ProductReviews
                .Where(r => r.ProductId == productId && r.Rating != null)
                .Select(r => (double)r.Rating!)
                .ToListAsync(ct);
            return ratings.Count == 0 ? 0d : Math.Round(ratings.Average(), 2);
        }

        public async Task<Dictionary<int, int>> RatingBreakdownByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var rows = await _context.ProductReviews
                .Where(r => r.ProductId == productId && r.Rating != null)
                .GroupBy(r => r.Rating!.Value)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var result = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };
            foreach (var r in rows)
            {
                if (r.Rating >= 1 && r.Rating <= 5) result[r.Rating] = r.Count;
            }
            return result;
        }

        public async Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken ct = default)
            => await _context.ProductReviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId, ct);

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId, CancellationToken ct = default)
        {
            // User A mua sản phẩm A → có OrderItem với ProductId=A trong Order của user A
            // và Order ở trạng thái "Delivered" hoặc "Completed".
            return await _context.OrderItems
                .AnyAsync(oi => oi.ProductId == productId
                                && oi.Order.UserId == userId
                                && PurchasedStatuses.Contains(oi.Order.Status), ct);
        }

        public async Task<string?> GetUserLoyaltyTierNameAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserLoyalty
                .Where(ul => ul.UserId == userId)
                .Select(ul => ul.Tier.Name)
                .FirstOrDefaultAsync(ct);
        }
    }
}
