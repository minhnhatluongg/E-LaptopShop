using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    /// <summary>
    /// Logic Loyalty:
    /// - Khi đơn chuyển Delivered/Completed: cộng điểm theo công thức và LifetimeSpend.
    /// - Re-evaluate tier dựa trên LifetimeSpend (Đồng→Bạc→Vàng→Bạch Kim).
    /// - Nếu là đơn đầu tiên thành công → cấp coupon WELCOME10.
    ///
    /// Quy đổi điểm: 1.000đ → 1 điểm × PointsMultiplier của tier hiện tại.
    /// </summary>
    public interface ILoyaltyService
    {
        /// <summary>
        /// Award điểm + cập nhật tier cho user dựa trên 1 order vừa Delivered/Completed.
        /// Idempotent: kiểm tra PointTransaction theo OrderId, đã có thì bỏ qua.
        /// </summary>
        Task AwardForDeliveredOrderAsync(int orderId, CancellationToken ct = default);
    }
}
