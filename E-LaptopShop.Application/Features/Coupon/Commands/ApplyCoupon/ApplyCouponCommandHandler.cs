using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.ApplyCoupon
{
    public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, ApplyCouponResultDto>
    {
        private readonly ICouponService _couponService;
        public ApplyCouponCommandHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<ApplyCouponResultDto> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
            => await _couponService.ValidateAsync(request.Code, request.OrderAmount, request.UserId, cancellationToken);
    }
}
