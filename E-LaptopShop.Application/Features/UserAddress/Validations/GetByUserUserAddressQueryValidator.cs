using E_LaptopShop.Application.Features.UserAddress.Queries.GetByUser;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class GetByUserUserAddressQueryValidator : AbstractValidator<GetUserAddressesQuery>
    {
        public GetByUserUserAddressQueryValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
