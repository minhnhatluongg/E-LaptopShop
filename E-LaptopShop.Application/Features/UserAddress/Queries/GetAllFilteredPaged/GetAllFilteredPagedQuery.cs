using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetAllFilteredPaged
{
    public class GetAllFilteredPagedQuery : BasePagedQuery<UserAddressDto>
    {
        public int? UserId { get; init; }
        public bool? IsDefault { get; init; }
        public string? CountryCode { get; init; } = "VN";
        public bool? IsDeleted { get; init; } = false;
        public string? Ward { get; init; }
        public string? City { get; init; }

        public DateTimeOffset? CreatedFrom { get; init; }
        public DateTimeOffset? CreatedTo { get; init; }
        public DateTimeOffset? UpdatedFrom { get; init; }
        public DateTimeOffset? UpdatedTo { get; init; }

        public bool IncludeUser { get; init; } = false;
    }
}
