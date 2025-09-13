using E_LaptopShop.Application.Features.UserAddress.Commands.SetDefaultUserAddress;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class SetDefaultUserAddressCommandValidator : AbstractValidator<SetDefaultUserAddressCommand>
    {
        public SetDefaultUserAddressCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);      
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
