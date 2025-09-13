using E_LaptopShop.Application.Features.UserAddress.Queries.GetDefaultByUser;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class GetDefaultByUserQueryValidator : AbstractValidator<GetDefaultByUserQuery>
    {
        public GetDefaultByUserQueryValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
