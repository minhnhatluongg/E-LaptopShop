using E_LaptopShop.Application.Services.Interfaces;
using MediatR;

namespace E_LaptopShop.Application.Features.Coupon.Commands.DeleteCoupon
{
    public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, bool>
    {
        private readonly ICouponService _couponService;
        public DeleteCouponCommandHandler(ICouponService couponService) => _couponService = couponService;

        public async Task<bool> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
            => await _couponService.DeleteAsync(request.Id, cancellationToken);
    }
}
