using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        public ClearCartCommandHandler(
            IShoppingCartRepository cartRepository,
            IShoppingCartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }
        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
            {
                return false;
            }
            var result = await _cartItemRepository.DeleteByCartIdAsync(cart.Id, cancellationToken);
            if (result > 0)
            {
                // Reset tổng tiền về 0
                cart.TotalAmount = 0;
                await _cartRepository.UpdateAsync(cart, cancellationToken);
                return true;
            }
            return false;
        }
    }
}
