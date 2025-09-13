using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetExist
{
    public class ExistsByUserPhoneQuery : IRequest<bool>
    {
        public int UserId { get; init; }
        public string? AddressLine { get; init; } = null!;
        public string Phone { get; init; } = null!;
    }
}
