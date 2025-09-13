using E_LaptopShop.Application.Features.UserAddress.Queries.GetAllFilteredPaged;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class GetAllFilteredPagedQueryValidator : AbstractValidator<GetAllFilteredPagedQuery>
    {
        private static readonly string[] AllowedSortBy =
        ["FullName", "City", "District", "Ward", "IsDefault", "CreatedAt", "UpdatedAt"];

        public GetAllFilteredPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

            RuleFor(x => x.SortBy!)
                .Must(s => string.IsNullOrWhiteSpace(s) || AllowedSortBy.Contains(s))
                .WithMessage("SortBy không hợp lệ.");
        }
    }
}
