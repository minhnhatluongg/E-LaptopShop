using FluentValidation;
using E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

namespace E_LaptopShop.Application.Features.Products.Validations;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to 0")
            .LessThanOrEqualTo(100).WithMessage("Discount must be less than or equal to 100")
            .When(x => x.Discount.HasValue);

        RuleFor(x => x.InStock)
            .GreaterThanOrEqualTo(0).WithMessage("InStock must be greater than or equal to 0")
            .When(x => x.InStock.HasValue);
    }
} 