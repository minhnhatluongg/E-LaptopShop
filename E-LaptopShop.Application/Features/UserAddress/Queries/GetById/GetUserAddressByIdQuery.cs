using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetById
{
    public class GetUserAddressByIdQuery : IRequest<UserAddressDto>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
