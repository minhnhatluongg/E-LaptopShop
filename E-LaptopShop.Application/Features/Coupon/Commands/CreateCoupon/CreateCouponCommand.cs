using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.CreateCoupon
{
    public class CreateCouponCommand : IRequest<CouponDto>
    {
        public CreateCouponRequestDto RequestDto { get; init; } = null!;
    }
}
