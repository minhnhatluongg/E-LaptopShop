using E_LaptopShop.Application.Features.Coupon.Commands.UpdateCoupon;
using FluentValidation;

namespace E_LaptopShop.Application.Features.Coupon.Validations
{
    public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
    {
        public UpdateCouponCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("ID không hợp lệ.");
            RuleFor(x => x.RequestDto).NotNull();

            RuleFor(x => x.RequestDto.DiscountType)
                .Must(t => t == null || t == "percent" || t == "fixed")
                .WithMessage("DiscountType chỉ chấp nhận 'percent' hoặc 'fixed'.");

            RuleFor(x => x.RequestDto.DiscountValue)
                .GreaterThan(0)
                .When(x => x.RequestDto.DiscountValue.HasValue)
                .WithMessage("DiscountValue phải > 0.");

            RuleFor(x => x.RequestDto.MinOrderAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.RequestDto.MinOrderAmount.HasValue);
        }
    }
}
