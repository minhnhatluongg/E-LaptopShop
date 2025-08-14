using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.UpdateCartItem
{
    public class UpdateCartItemCommand : IRequest<ShoppingCartItemDto>
    {
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
