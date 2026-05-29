using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Services
{
    /// <summary>
    /// Implementation Loyalty — trực tiếp truy cập DbContext.
    /// Đặt ở Infra layer vì cần ApplicationDbContext.
    /// </summary>
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LoyaltyService> _logger;

        // Quy đổi: 1.000đ chi tiêu → 1 điểm (base, trước khi nhân multiplier)
        private const decimal POINT_DIVISOR = 1000m;

        public LoyaltyService(ApplicationDbContext db, ILogger<LoyaltyService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task AwardForDeliveredOrderAsync(int orderId, CancellationToken ct = default)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
            if (order == null || order.UserId == null || order.TotalAmount <= 0)
            {
                _logger.LogDebug("[Loyalty] Skip — order {OrderId} missing data", orderId);
                return;
            }

            // Idempotent: nếu đã từng cấp điểm cho order này → bỏ qua.
            var already = await _db.PointTransactions
                .AnyAsync(pt => pt.OrderId == orderId && pt.Points > 0, ct);
            if (already)
            {
                _logger.LogDebug("[Loyalty] Order {OrderId} đã được award trước đó", orderId);
                return;
            }

            var userId = order.UserId.Value;

            // Lấy hoặc tạo UserLoyalty (mặc định tier Đồng)
            var loyalty = await _db.UserLoyalty
                .FirstOrDefaultAsync(ul => ul.UserId == userId, ct);

            if (loyalty == null)
            {
                var bronzeTierId = await _db.LoyaltyTiers
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.MinSpend)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync(ct);

                if (bronzeTierId == null)
                {
                    _logger.LogWarning("[Loyalty] Chưa có LoyaltyTier nào — seed dữ liệu trước.");
                    return;
                }

                loyalty = new UserLoyalty
                {
                    UserId = userId,
                    TierId = bronzeTierId.Value,
                    TotalPoints = 0,
                    LifetimeSpend = 0,
                    UpdatedAt = DateTime.UtcNow,
                };
                _db.UserLoyalty.Add(loyalty);
                await _db.SaveChangesAsync(ct);
            }

            // Tính điểm: base = TotalAmount / 1.000, sau đó × PointsMultiplier
            var currentTier = await _db.LoyaltyTiers
                .FirstOrDefaultAsync(t => t.Id == loyalty.TierId, ct);
            var multiplier = currentTier?.PointsMultiplier ?? 1m;
            var earnedPoints = (int)Math.Floor(order.TotalAmount / POINT_DIVISOR * multiplier);

            // Cộng điểm + LifetimeSpend
            loyalty.TotalPoints   += earnedPoints;
            loyalty.LifetimeSpend += order.TotalAmount;
            loyalty.UpdatedAt      = DateTime.UtcNow;

            // Re-evaluate tier (chỉ tăng, không giảm)
            var newTier = await _db.LoyaltyTiers
                .Where(t => t.IsActive && t.MinSpend <= loyalty.LifetimeSpend)
                .OrderByDescending(t => t.MinSpend)
                .FirstOrDefaultAsync(ct);
            if (newTier != null && newTier.Id != loyalty.TierId)
            {
                _logger.LogInformation(
                    "[Loyalty] User {UserId} thăng hạng: TierId {Old} → {New} (LifetimeSpend={Spend})",
                    userId, loyalty.TierId, newTier.Id, loyalty.LifetimeSpend);
                loyalty.TierId = newTier.Id;
            }

            // Log giao dịch điểm
            _db.PointTransactions.Add(new PointTransaction
            {
                UserId    = userId,
                OrderId   = orderId,
                Points    = earnedPoints,
                Reason    = $"Hoàn thành đơn #{order.OrderNumber} (+{earnedPoints} điểm)",
                CreatedAt = DateTime.UtcNow,
            });

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "[Loyalty] User {UserId} +{Points} điểm, LifetimeSpend={Spend} cho order {OrderId}",
                userId, earnedPoints, loyalty.LifetimeSpend, orderId);

            // Nếu đây là đơn HOÀN THÀNH đầu tiên → cấp coupon WELCOME10
            await GrantWelcomeCouponIfFirstOrderAsync(userId, ct);
        }

        private async Task GrantWelcomeCouponIfFirstOrderAsync(int userId, CancellationToken ct)
        {
            // Đếm số order đã Delivered/Completed của user
            var completedCount = await _db.Orders
                .CountAsync(o => o.UserId == userId
                              && (o.Status == "Delivered" || o.Status == "Completed"), ct);
            if (completedCount != 1) return; // chỉ cấp ở đơn đầu

            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == "WELCOME10" && c.IsActive, ct);
            if (coupon == null)
            {
                _logger.LogDebug("[Loyalty] Coupon WELCOME10 không tồn tại — bỏ qua.");
                return;
            }

            // Đã dùng / đã cấp cho user này chưa?
            var alreadyUsed = await _db.CouponUsages
                .AnyAsync(cu => cu.UserId == userId && cu.CouponId == coupon.Id, ct);
            if (alreadyUsed) return;

            // Tạo notification welcome (không bắt buộc, có thì tạo)
            try
            {
                _db.Notifications.Add(new Notification
                {
                    UserId    = userId,
                    Title     = "🎁 Bạn được tặng mã giảm giá WELCOME10",
                    Body      = "Cảm ơn bạn đã mua hàng! Dùng mã WELCOME10 ở đơn tiếp theo để được giảm 10% (tối đa 500k).",
                    Type      = "Coupon",
                    IsRead    = false,
                    CreatedAt = DateTime.UtcNow,
                });
                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Loyalty] Không tạo được notification welcome");
            }
        }
    }
}
