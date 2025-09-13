using E_LaptopShop.Application.Features.UserAddress.Commands.DeleteUserAddress;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class DeleteUserAddressCommandValidator : AbstractValidator<DeleteUserAddressCommand>
    {
        public DeleteUserAddressCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
