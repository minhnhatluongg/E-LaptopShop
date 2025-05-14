using E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Validations
{
    public class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
    {
        public UpdateProductImageCommandValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.ImageUrl).NotEmpty();
            RuleFor(x => x.FileType).NotEmpty();
            RuleFor(x => x.FileSize).GreaterThan(0);
        }
    }
}
