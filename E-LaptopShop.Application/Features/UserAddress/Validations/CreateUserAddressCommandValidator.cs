using E_LaptopShop.Application.Features.UserAddress.Commands.CreateUserAddress;
using static E_LaptopShop.Application.Features.UserAddress.Validations.AddressRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class CreateUserAddressCommandValidator : AbstractValidator<CreateUserAddressCommand>
    {
        public CreateUserAddressCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);

            RuleFor(x => x.FullName)
                .NotEmpty().MaximumLength(MaxName);

            RuleFor(x => x.Phone)
                .NotEmpty().MaximumLength(MaxPhone)
                .MustBeVietnamPhone();

            RuleFor(x => x.AddressLine)
                .NotEmpty().MaximumLength(MaxLine);

            RuleFor(x => x.City).NotEmpty().MaximumLength(MaxCity);
            RuleFor(x => x.District).NotEmpty().MaximumLength(MaxDistrict);
            RuleFor(x => x.Ward).NotEmpty().MaximumLength(MaxWard);

            RuleFor(x => x.CountryCode)
                .NotEmpty().Length(2, 3);

            RuleFor(x => x.PostalCode)
                .MaximumLength(MaxPostal).When(x => !string.IsNullOrWhiteSpace(x.PostalCode));
        }
    }
}
