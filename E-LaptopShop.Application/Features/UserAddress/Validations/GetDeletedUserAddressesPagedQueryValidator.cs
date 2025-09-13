using E_LaptopShop.Application.Features.UserAddress.Queries.GetDeletedHavePaged;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public class GetDeletedUserAddressesPagedQueryValidator : AbstractValidator<GetDeletedUserAddressesPagedQuery>
    {
        private static readonly string[] AllowedSortBy =
            ["FullName", "City", "District", "Ward", "IsDefault", "CreatedAt", "UpdatedAt", "DeletedAt"];

        public GetDeletedUserAddressesPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

            RuleFor(x => x.SortBy!)
                .Must(s => string.IsNullOrWhiteSpace(s) || AllowedSortBy.Contains(s))
                .WithMessage("SortBy không hợp lệ.");
            //RuleFor(x => x.SortOrder!)
            //    .Must(o => string.IsNullOrWhiteSpace(o) || o.Equals("asc", true) || o.Equals("desc", true))
            //    .WithMessage("SortOrder chỉ nhận 'asc' hoặc 'desc'.");
        }
    }
}
