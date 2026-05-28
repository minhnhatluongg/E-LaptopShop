using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Coupon.Commands.CreateCoupon
{
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, CouponDto>
    {
        private readonly ICouponService _couponService;
        private readonly ILogger<CreateCouponCommandHandler> _logger;

        public CreateCouponCommandHandler(ICouponService couponService, ILogger<CreateCouponCommandHandler> logger)
        {
            _couponService = couponService;
            _logger = logger;
        }

        public async Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating coupon with code {Code}", request.RequestDto.Code);
            return await _couponService.CreateAsync(request.RequestDto, cancellationToken);
        }
    }
}
