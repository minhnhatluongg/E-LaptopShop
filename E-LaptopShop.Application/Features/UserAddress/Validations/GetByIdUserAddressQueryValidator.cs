using E_LaptopShop.Application.Features.UserAddress.Queries.GetById;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class GetByIdUserAddressQueryValidator : AbstractValidator<GetUserAddressByIdQuery>
    {
        public GetByIdUserAddressQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
