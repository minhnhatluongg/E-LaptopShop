using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetCouponById
{
    public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, CouponDto?>
    {
        private readonly ICouponService _couponService;
        public GetCouponByIdQueryHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<CouponDto?> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
            => await _couponService.GetByIdAsync(request.Id, cancellationToken);
    }
}
