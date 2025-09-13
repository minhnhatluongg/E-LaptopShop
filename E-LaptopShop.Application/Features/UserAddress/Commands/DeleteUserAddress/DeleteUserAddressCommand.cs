using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.DeleteUserAddress
{
    public class DeleteUserAddressCommand : IRequest<int>
    {
        public int Id { get; set; }
    }
}
