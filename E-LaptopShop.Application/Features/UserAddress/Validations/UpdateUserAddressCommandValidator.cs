using E_LaptopShop.Application.Features.UserAddress.Commands.UpdateUserAddress;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
    {
        public UpdateUserAddressCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("ID must be greater than 0");
            RuleFor(x => x.UserId).GreaterThan(0);

            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
            RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(255);

            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Ward).NotEmpty().MaximumLength(100);

            RuleFor(x => x.CountryCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.PostalCode).MaximumLength(20);
        }
    }
}
