using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetAllCoupons
{
    public class GetAllCouponsQuery : IRequest<PagedResult<CouponDto>>
    {
        public CouponQueryParams QueryParams { get; init; } = new CouponQueryParams();
    }
}
