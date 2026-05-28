using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.UpdateCoupon
{
    public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, CouponDto>
    {
        private readonly ICouponService _couponService;
        public UpdateCouponCommandHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<CouponDto> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
            => await _couponService.UpdateAsync(request.Id, request.RequestDto, cancellationToken);
    }
}
