using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.DeleteCoupon
{
    public class DeleteCouponCommand : IRequest<bool>
    {
        public int Id { get; init; }
    }
}
