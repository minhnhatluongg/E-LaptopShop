using FluentValidation;
using E_LaptopShop.Application.Features.Products.Commands.CreateProduct;

namespace E_LaptopShop.Application.Features.Products.Validations;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.RequestDto.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.RequestDto.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.RequestDto.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to 0")
            .LessThanOrEqualTo(100).WithMessage("Discount must be less than or equal to 100")
            .When(x => x.RequestDto.Discount.HasValue);

        RuleFor(x => x.RequestDto.InStock)
            .GreaterThanOrEqualTo(0).WithMessage("InStock must be greater than or equal to 0");

        RuleFor(x => x.RequestDto.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than 0");
    }
} 