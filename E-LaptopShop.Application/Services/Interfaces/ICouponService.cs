using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Base;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface ICouponService
        : IBaseService<CouponDto, CreateCouponRequestDto, UpdateCouponRequestDto, CouponQueryParams>
    {
        Task<CouponDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate coupon for a given order amount + user. Throws on invalid.
        /// Returns the calculated discount + final amount. Does NOT consume the coupon.
        /// </summary>
        Task<ApplyCouponResultDto> ValidateAsync(
            string code,
            decimal orderAmount,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Atomically validate + consume + record CouponUsage. Used at order placement.
        /// </summary>
        Task<ApplyCouponResultDto> RedeemAsync(
            string code,
            decimal orderAmount,
            int userId,
            int? orderId,
            CancellationToken cancellationToken = default);
    }
}
