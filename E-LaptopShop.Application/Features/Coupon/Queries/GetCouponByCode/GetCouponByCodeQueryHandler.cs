using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetCouponByCode
{
    public class GetCouponByCodeQueryHandler : IRequestHandler<GetCouponByCodeQuery, CouponDto?>
    {
        private readonly ICouponService _couponService;
        public GetCouponByCodeQueryHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<CouponDto?> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
            => await _couponService.GetByCodeAsync(request.Code, cancellationToken);
    }
}
