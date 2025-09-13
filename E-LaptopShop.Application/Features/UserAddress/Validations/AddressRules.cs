using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Validations
{
    public static class AddressRules
    {
        public const int MaxName = 150;
        public const int MaxPhone = 20;
        public const int MaxLine = 250;
        public const int MaxCity = 120;
        public const int MaxDistrict = 120;
        public const int MaxWard = 120;
        public const int MaxPostal = 20;
        public const string DefaultCountry = "VN";

        // Regex VN: 0xxxxxxxxx | +84xxxxxxxxx (9–10 digits sau đầu số)
        public const string VnPhoneRegex = @"^(0|\+84)(\d{9,10})$";

        public static IRuleBuilderOptions<T, string> MustBeVietnamPhone<T>(this IRuleBuilder<T, string> rule)
            => rule.Matches(VnPhoneRegex).WithMessage("Số điện thoại không hợp lệ (VN).");
    }
}
