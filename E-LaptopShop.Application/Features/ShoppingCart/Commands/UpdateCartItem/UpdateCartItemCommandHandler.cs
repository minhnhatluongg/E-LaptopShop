using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, ShoppingCartItemDto>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;

        public UpdateCartItemCommandHandler(
        IShoppingCartRepository cartRepository,
        IShoppingCartItemRepository cartItemRepository,
        IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        public async Task<ShoppingCartItemDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(request.ItemId, cancellationToken);
            if (cartItem == null)
            {
                throw new KeyNotFoundException($"Cart item with ID {request.ItemId} not found.");
            }

            // Kiểm tra quyền sở hữu
            if (cartItem.ShoppingCart.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You don't have permission to update this cart item.");
            }

            if (request.Quantity <= 0)
            {
                // Xóa item nếu quantity <= 0
                await _cartItemRepository.DeleteAsync(request.ItemId, cancellationToken);
                
                // Cập nhật tổng tiền của cart
                await UpdateCartTotal(cartItem.ShoppingCartId, cancellationToken);
                
                return null; // Trả về null để báo hiệu item đã bị xóa
            }

            cartItem.Quantity = request.Quantity;
            cartItem = await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);

            // Cập nhật tổng tiền của cart
            await UpdateCartTotal(cartItem.ShoppingCartId, cancellationToken);

            return _mapper.Map<ShoppingCartItemDto>(cartItem);
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
