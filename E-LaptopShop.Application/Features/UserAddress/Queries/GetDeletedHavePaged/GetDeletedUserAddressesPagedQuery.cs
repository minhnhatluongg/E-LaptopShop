using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetDeletedHavePaged
{
    public class GetDeletedUserAddressesPagedQuery : BasePagedQuery<UserAddressDto>
    {
        public int? UserId { get; init; }
    }
}
