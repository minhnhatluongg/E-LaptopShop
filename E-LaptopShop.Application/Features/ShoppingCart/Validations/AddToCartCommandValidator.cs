using E_LaptopShop.Application.Features.ShoppingCart.Commands.CreateShoppingCard;
using FluentValidation;

namespace E_LaptopShop.Application.Features.ShoppingCart.Validations
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID must be greater than 0.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Product ID must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Quantity cannot exceed 100.");
        }
    }
}
