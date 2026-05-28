using E_LaptopShop.Application.Features.Coupon.Commands.CreateCoupon;
using FluentValidation;

namespace E_LaptopShop.Application.Features.Coupon.Validations
{
    public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
    {
        public CreateCouponCommandValidator()
        {
            RuleFor(x => x.RequestDto)
                .NotNull()
                .WithMessage("Dữ liệu tạo coupon không được để trống.");

            RuleFor(x => x.RequestDto.Code)
                .NotEmpty().WithMessage("Mã coupon là bắt buộc.")
                .MaximumLength(50).WithMessage("Mã coupon không được vượt quá 50 ký tự.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Mã coupon chỉ chấp nhận chữ in hoa, số, dấu '-' hoặc '_'.");

            RuleFor(x => x.RequestDto.DiscountType)
                .NotEmpty()
                .Must(t => t == "percent" || t == "fixed")
                .WithMessage("DiscountType chỉ chấp nhận 'percent' hoặc 'fixed'.");

            RuleFor(x => x.RequestDto.DiscountValue)
                .GreaterThan(0).WithMessage("DiscountValue phải > 0.");

            When(x => x.RequestDto.DiscountType == "percent", () =>
            {
                RuleFor(x => x.RequestDto.DiscountValue)
                    .LessThanOrEqualTo(100)
                    .WithMessage("Phần trăm giảm tối đa là 100.");
            });

            RuleFor(x => x.RequestDto.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MinOrderAmount không hợp lệ.");

            RuleFor(x => x.RequestDto.MaxDiscountAmount)
                .GreaterThan(0)
                .When(x => x.RequestDto.MaxDiscountAmount.HasValue)
                .WithMessage("MaxDiscountAmount phải > 0.");

            RuleFor(x => x.RequestDto.UsageLimit)
                .GreaterThan(0)
                .When(x => x.RequestDto.UsageLimit.HasValue)
                .WithMessage("UsageLimit phải > 0.");

            RuleFor(x => x.RequestDto.UsageLimitPerUser)
                .GreaterThan(0)
                .When(x => x.RequestDto.UsageLimitPerUser.HasValue)
                .WithMessage("UsageLimitPerUser phải > 0.");

            RuleFor(x => x.RequestDto.EndsAt)
                .GreaterThan(x => x.RequestDto.StartsAt)
                .When(x => x.RequestDto.EndsAt.HasValue)
                .WithMessage("EndsAt phải sau StartsAt.");
        }
    }
}
