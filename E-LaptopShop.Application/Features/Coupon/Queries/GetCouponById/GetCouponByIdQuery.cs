using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Queries.GetCouponById
{
    public class GetCouponByIdQuery : IRequest<CouponDto?>
    {
        public int Id { get; init; }
    }
}
