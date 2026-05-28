using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetCouponByCode
{
    public class GetCouponByCodeQuery : IRequest<CouponDto?>
    {
        public string Code { get; init; } = null!;
    }
}
