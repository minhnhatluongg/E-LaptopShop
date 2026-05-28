using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.ApplyCoupon
{
    /// <summary>
    /// Validate-only. Returns calculated discount without consuming the coupon.
    /// Used at checkout preview before order placement.
    /// </summary>
    public class ApplyCouponCommand : IRequest<ApplyCouponResultDto>
    {
        public string Code { get; init; } = null!;
        public decimal OrderAmount { get; init; }
        public int UserId { get; init; }
    }
}
