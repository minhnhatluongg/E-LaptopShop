using E_LaptopShop.Application.Features.ProductImage.Commands.SetMainImage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Validations
{
    public class SetMainImageCommandValidator : AbstractValidator<SetMainImageCommand>
    {
        public SetMainImageCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0");
        }
    }
}
