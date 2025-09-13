using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetStatics
{
    public class GetUserAddressStatsQuery : IRequest<UserAddressStatsDto>
    {
        public int? UserId { get; init; }
        public string? CountryCode { get; init; } = "VN";
    }

    public sealed class UserAddressStatsDto
    {
        public int Total { get; init; }
        public int Active { get; init; }
        public int Deleted { get; init; }
        public int DefaultCount { get; init; }
    }
}
