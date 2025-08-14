using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}
