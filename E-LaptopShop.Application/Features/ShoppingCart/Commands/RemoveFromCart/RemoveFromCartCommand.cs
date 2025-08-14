using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.RemoveFromCart
{
    public class RemoveFromCartCommand : IRequest<bool>
    {
        public int ItemId { get; set; }
        public int UserId { get; set; }
    }
}
