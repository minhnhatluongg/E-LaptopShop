using FluentValidation;
using E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;

namespace E_LaptopShop.Application.Features.Categories.Validations;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
} 