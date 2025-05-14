using E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Validations
{
    public class CreateProductImageCommandValidators : AbstractValidator<CreateProductImageCommand>
    {
        public CreateProductImageCommandValidators()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.ImageUrl).NotEmpty();
            RuleFor(x => x.FileType).NotEmpty();
            RuleFor(x => x.FileSize).GreaterThan(0);
        }
    }
}
