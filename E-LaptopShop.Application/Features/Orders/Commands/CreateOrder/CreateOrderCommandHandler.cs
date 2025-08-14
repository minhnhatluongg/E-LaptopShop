using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IShoppingCartRepository cartRepository,
            IShoppingCartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra user tồn tại
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            // Lấy items để tạo order
            List<(Product Product, int Quantity, decimal UnitPrice)> orderItems;
            
            if (request.CreateFromCart)
            {
                orderItems = await GetItemsFromCart(request.UserId, cancellationToken);
            }
            else
            {
                orderItems = await GetItemsFromCommand(request.Items, cancellationToken);
            }

            if (!orderItems.Any())
            {
                throw new InvalidOperationException("Cannot create order with no items");
            }

            // Tạo order number unique
            var orderNumber = await GenerateOrderNumber();

            // Tính toán các giá trị
            var subTotal = orderItems.Sum(x => x.UnitPrice * x.Quantity);
            var discountAmount = await CalculateDiscount(request.DiscountCode, subTotal);
            var taxAmount = CalculateTax(subTotal - discountAmount);
            var shippingFee = CalculateShippingFee(request.ShippingMethod, subTotal);
            var totalAmount = subTotal - discountAmount + taxAmount + shippingFee;

            // Tạo order
            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = request.UserId,
                ShippingAddressId = request.ShippingAddressId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending.ToString(),
                SubTotal = subTotal,
                DiscountAmount = discountAmount,
                DiscountCode = request.DiscountCode,
                TaxAmount = taxAmount,
                ShippingFee = shippingFee,
                TotalAmount = totalAmount,
                ShippingMethod = request.ShippingMethod,
                PaymentMethod = request.PaymentMethod,
                IsPaid = false,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId.ToString()
            };

            // Lưu order
            order = await _orderRepository.CreateAsync(order, cancellationToken);

            // Tạo order items
            foreach (var item in orderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    CostPrice = 0, // TODO: Add CostPrice to Product entity
                    DiscountAmount = 0, // Tính theo từng item nếu cần
                    DiscountPercent = 0,
                    TaxAmount = 0, // Tính theo từng item nếu cần
                    SubTotal = item.UnitPrice * item.Quantity,
                    SKU = null, // TODO: Add SKU to Product entity
                    Status = OrderItemStatus.Pending.ToString()
                };

                await _orderItemRepository.CreateAsync(orderItem, cancellationToken);
            }

            // Xóa giỏ hàng nếu tạo từ cart
            if (request.CreateFromCart)
            {
                await ClearCartAfterOrder(request.UserId, cancellationToken);
            }

            // Load lại order với đầy đủ thông tin
            var createdOrder = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
            return _mapper.Map<OrderDto>(createdOrder);
        }

        private async Task<List<(Product Product, int Quantity, decimal UnitPrice)>> GetItemsFromCart(int userId, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(userId, cancellationToken);
            if (cart?.Items == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Shopping cart is empty");
            }

            var items = new List<(Product, int, decimal)>();
            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId, cancellationToken);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {cartItem.ProductId} not found");
                }

                // Kiểm tra stock
                if (product.InStock < cartItem.Quantity)
                {
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {product.InStock}, Requested: {cartItem.Quantity}");
                }

                items.Add((product, cartItem.Quantity, cartItem.UnitPrice));
            }

            return items;
        }

        private async Task<List<(Product Product, int Quantity, decimal UnitPrice)>> GetItemsFromCommand(List<CreateOrderItemDto> items, CancellationToken cancellationToken)
        {
            var orderItems = new List<(Product, int, decimal)>();
            
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");
                }

                // Kiểm tra stock
                if (product.InStock < item.Quantity)
                {
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}");
                }

                var unitPrice = item.CustomPrice ?? product.Price;
                orderItems.Add((product, item.Quantity, unitPrice));
            }

            return orderItems;
        }

        private async Task<string> GenerateOrderNumber()
        {
            var prefix = "ORD";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100, 999);
            return $"{prefix}{timestamp}{random}";
        }

        private async Task<decimal> CalculateDiscount(string discountCode, decimal subTotal)
        {
            // TODO: Implement discount code logic
            return 0;
        }

        private decimal CalculateTax(decimal amount)
        {
            // TODO: Implement tax calculation (VAT 10%)
            return amount * 0.1m;
        }

        private decimal CalculateShippingFee(string shippingMethod, decimal subTotal)
        {
            // TODO: Implement shipping fee calculation
            return shippingMethod?.ToLower() switch
            {
                "express" => 50000,
                "standard" => 30000,
                "free" when subTotal >= 500000 => 0,
                _ => 30000
            };
        }

        private async Task ClearCartAfterOrder(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
                if (cart != null)
                {
                    await _cartItemRepository.DeleteByCartIdAsync(cart.Id, cancellationToken);
                }
            }
            catch
            {
                // Không throw exception nếu clear cart fail
            }
        }
    }
}
