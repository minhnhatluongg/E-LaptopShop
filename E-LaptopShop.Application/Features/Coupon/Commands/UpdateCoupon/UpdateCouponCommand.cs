using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.UpdateCoupon
{
    public sealed record UpdateCouponCommand : IRequest<CouponDto>
    {
        public int Id { get; init; }
        public UpdateCouponRequestDto RequestDto { get; init; } = null!;
    }
}
