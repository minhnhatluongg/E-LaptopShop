using E_LaptopShop.Application.Features.UserAddress.Commands.HardDeleteUserAddress;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class HardDeleteUserAddressCommandValidator : AbstractValidator<hardDeleteUserAddressCommand>
    {
        public HardDeleteUserAddressCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
