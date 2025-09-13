using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.SetDefaultUserAddress
{
    public class SetDefaultUserAddressCommand : IRequest
    {
        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
