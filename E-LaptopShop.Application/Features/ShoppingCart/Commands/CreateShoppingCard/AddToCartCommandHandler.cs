using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Commands.CreateShoppingCard
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ShoppingCartItemDto>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        
        public AddToCartCommandHandler(
            IShoppingCartRepository cartRepository,
            IShoppingCartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ShoppingCartItemDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra sản phẩm có tồn tại không
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
            }
            // Lấy hoặc tạo cart cho user
            var cart = await _cartRepository.GetCartWithItemsAsync(request.UserId, cancellationToken);

            // Kiểm tra sản phẩm đã có trong cart chưa
            var existingItem = await _cartItemRepository.GetByCartAndProductAsync(cart.Id, request.ProductId, cancellationToken);

            ShoppingCartItem cartItem;
            if (existingItem != null)
            {
                // Cập nhật số lượng
                existingItem.Quantity += request.Quantity;
                cartItem = await _cartItemRepository.UpdateAsync(existingItem, cancellationToken);
            }
            else
            {
                // Thêm item mới
                cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price,
                    AddedAt = DateTime.UtcNow
                };
                cartItem = await _cartItemRepository.AddAsync(cartItem, cancellationToken);
            }

            // Cập nhật tổng tiền của cart
            await UpdateCartTotal(cart.Id, cancellationToken);

            // Load lại cart item với thông tin product
            cartItem = await _cartItemRepository.GetByIdAsync(cartItem.Id, cancellationToken);

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
