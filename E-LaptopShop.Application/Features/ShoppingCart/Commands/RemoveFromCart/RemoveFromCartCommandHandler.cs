using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, bool>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;

        public RemoveFromCartCommandHandler(
        IShoppingCartRepository cartRepository,
        IShoppingCartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<bool> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(request.ItemId, cancellationToken);
            if (cartItem == null)
            {
                return false;
            }

            // Kiểm tra quyền sở hữu
            if (cartItem.ShoppingCart.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You don't have permission to remove this cart item.");
            }

            var cartId = cartItem.ShoppingCartId;
            var result = await _cartItemRepository.DeleteAsync(request.ItemId, cancellationToken);

            if (result > 0)
            {
                // Cập nhật tổng tiền của cart
                await UpdateCartTotal(cartId, cancellationToken);
                return true;
            }

            return false;
        }

        private async Task UpdateCartTotal(int cartId, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId, cancellationToken);
            if (cart != null)
            {
                cart.TotalAmount = cart.Items.Sum(i => i.Quantity * i.UnitPrice);
                await _cartRepository.UpdateAsync(cart, cancellationToken);
            }
        }
    }
}
