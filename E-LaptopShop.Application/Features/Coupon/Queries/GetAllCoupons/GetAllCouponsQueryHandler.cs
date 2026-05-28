using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetAllCoupons
{
    public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, PagedResult<CouponDto>>
    {
        private readonly ICouponService _couponService;
        public GetAllCouponsQueryHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<PagedResult<CouponDto>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            var p = request.QueryParams;
            p.ValidateAndNormalize();
            p.ValidateBusinessRules();
            return await _couponService.GetAllAsync(p, cancellationToken);
        }
    }
}
