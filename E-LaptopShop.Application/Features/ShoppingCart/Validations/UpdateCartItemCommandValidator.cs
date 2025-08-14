using E_LaptopShop.Application.Features.ShoppingCart.Commands.UpdateCartItem;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Validations;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0)
            .WithMessage("Item ID must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater than or equal to 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100.");
    }
}