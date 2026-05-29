using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository            _orderRepository;
        private readonly IOrderItemRepository        _orderItemRepository;
        private readonly IShoppingCartRepository     _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        private readonly IProductRepository          _productRepository;
        private readonly IUserRepository             _userRepository;
        private readonly IInventoryService           _inventoryService;
        private readonly ICouponService              _couponService;
        private readonly IMapper                     _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IShoppingCartRepository cartRepository,
            IShoppingCartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IInventoryService inventoryService,
            ICouponService couponService,
            IMapper mapper,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository     = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository      = cartRepository;
            _cartItemRepository  = cartItemRepository;
            _productRepository   = productRepository;
            _userRepository      = userRepository;
            _inventoryService    = inventoryService;
            _couponService       = couponService;
            _mapper              = mapper;
            _logger              = logger;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate user
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");

            // 2. Resolve order items (from cart or manual)
            var orderItems = request.CreateFromCart
                ? await GetItemsFromCart(request.UserId, cancellationToken)
                : await GetItemsFromCommand(request.Items, cancellationToken);

            if (orderItems.Count == 0)
                throw new InvalidOperationException("Không có sản phẩm nào trong đơn hàng");

            // 3. Stock check upfront (trước khi deduct, tránh partial deduction)
            foreach (var (product, qty, _) in orderItems)
            {
                var available = await _inventoryService.IsAvailableAsync(product.Id, qty, cancellationToken);
                if (!available)
                    throw new InvalidOperationException(
                        $"Sản phẩm \"{product.Name}\" không đủ hàng. Vui lòng cập nhật giỏ hàng.");
            }

            // 4. Calculate totals
            var subTotal      = orderItems.Sum(x => x.UnitPrice * x.Quantity);
            var discountAmount = 0m;
            var appliedCode   = (string?)null;

            // 4a. Apply coupon nếu có
            if (!string.IsNullOrWhiteSpace(request.DiscountCode))
            {
                try
                {
                    var couponResult = await _couponService.ValidateAsync(
                        request.DiscountCode, subTotal, request.UserId, cancellationToken);
                    discountAmount = couponResult.DiscountAmount;
                    appliedCode    = request.DiscountCode.Trim().ToUpper();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("[Order] Coupon invalid: {Msg}", ex.Message);
                    throw new InvalidOperationException($"Mã giảm giá không hợp lệ: {ex.Message}");
                }
            }

            var afterDiscount = subTotal - discountAmount;
            var taxAmount     = afterDiscount * 0.1m;         // VAT 10%
            var shippingFee   = ResolveShippingFee(request.ShippingMethod, subTotal);
            var totalAmount   = afterDiscount + taxAmount + shippingFee;
            var orderNumber   = GenerateOrderNumber();

            // 5. Persist order
            var order = new Order
            {
                OrderNumber       = orderNumber,
                UserId            = request.UserId,
                ShippingAddressId = request.ShippingAddressId,
                OrderDate         = DateTime.UtcNow,
                Status            = OrderStatus.Pending.ToString(),
                SubTotal          = subTotal,
                DiscountAmount    = discountAmount,
                DiscountCode      = appliedCode,
                TaxAmount         = taxAmount,
                ShippingFee       = shippingFee,
                TotalAmount       = totalAmount,
                ShippingMethod    = request.ShippingMethod,
                PaymentMethod     = request.PaymentMethod,
                IsPaid            = false,
                Notes             = request.Notes,
                CreatedAt         = DateTime.UtcNow,
                CreatedBy         = request.UserId.ToString(),
            };

            order = await _orderRepository.CreateAsync(order, cancellationToken);
            _logger.LogInformation("[Order] Created #{OrderNumber} UserId={UserId} Total={Total}",
                orderNumber, request.UserId, totalAmount);

            // 6. Persist order items + Deduct inventory (Optimistic Locking)
            var deductFailed = new List<string>();

            foreach (var (product, qty, unitPrice) in orderItems)
            {
                var item = new OrderItem
                {
                    OrderId        = order.Id,
                    ProductId      = product.Id,
                    Quantity       = qty,
                    UnitPrice      = unitPrice,
                    SubTotal       = unitPrice * qty,
                    Status         = OrderItemStatus.Pending.ToString(),
                    DiscountAmount = 0,
                    DiscountPercent = product.Discount ?? 0,
                };
                await _orderItemRepository.CreateAsync(item, cancellationToken);

                // Trừ kho với Optimistic Locking
                var (ok, newStock, msg) = await _inventoryService.DeductStockAsync(
                    product.Id, qty, order.Id,
                    $"Đặt hàng #{orderNumber}",
                    maxRetries: 3, ct: cancellationToken);

                if (!ok)
                {
                    deductFailed.Add($"{product.Name}: {msg}");
                    _logger.LogWarning("[Order] Deduct failed for product {Id}: {Msg}", product.Id, msg);
                }
                else
                {
                    _logger.LogInformation("[Order] Deducted stock product {Id} → {Stock}", product.Id, newStock);
                }
            }

            // Nếu có item không trừ được kho → log nhưng vẫn tạo đơn (Admin sẽ xử lý)
            // Trong production có thể cancel order nếu deductFailed.Count > 0
            if (deductFailed.Count > 0)
                _logger.LogError("[Order] #{OrderNumber} — Stock deduction failed: {Items}",
                    orderNumber, string.Join(", ", deductFailed));

            // 7. Consume coupon (ghi CouponUsage)
            if (!string.IsNullOrWhiteSpace(appliedCode))
            {
                try
                {
                    await _couponService.RedeemAsync(
                        appliedCode, subTotal, request.UserId, order.Id, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[Order] Coupon redeem failed for #{OrderNumber}", orderNumber);
                }
            }

            // 8. Clear cart if ordered from cart
            if (request.CreateFromCart)
                await ClearCartAsync(request.UserId, cancellationToken);

            // 9. Return full order
            var created = await _orderRepository.GetByIdAsync(order.Id, cancellationToken);
            return _mapper.Map<OrderDto>(created!);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task<List<(Product Product, int Quantity, decimal UnitPrice)>> GetItemsFromCart(
            int userId, CancellationToken ct)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(userId, ct);
            if (cart?.Items == null || cart.Items.Count == 0)
                throw new InvalidOperationException("Giỏ hàng trống");

            var result = new List<(Product, int, decimal)>();
            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct)
                    ?? throw new KeyNotFoundException($"Sản phẩm #{item.ProductId} không tìm thấy");

                result.Add((product, item.Quantity, item.UnitPrice));
            }
            return result;
        }

        private async Task<List<(Product Product, int Quantity, decimal UnitPrice)>> GetItemsFromCommand(
            List<CreateOrderItemDto> items, CancellationToken ct)
        {
            var result = new List<(Product, int, decimal)>();
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct)
                    ?? throw new KeyNotFoundException($"Sản phẩm #{item.ProductId} không tìm thấy");

                result.Add((product, item.Quantity, item.CustomPrice ?? product.Price));
            }
            return result;
        }

        private static string GenerateOrderNumber()
        {
            var rand = System.Security.Cryptography.RandomNumberGenerator.GetInt32(10000, 99999);
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmm}{rand:D5}";
        }

        private static decimal ResolveShippingFee(string? method, decimal subTotal) =>
            method?.ToLower() switch
            {
                "express"  => 50_000m,
                "free"     => 0m,
                "standard" => subTotal >= 10_000_000m ? 0m : 30_000m, // Free ship đơn ≥10tr
                _          => subTotal >= 10_000_000m ? 0m : 30_000m,
            };

        private async Task ClearCartAsync(int userId, CancellationToken ct)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId, ct);
                if (cart != null)
                    await _cartItemRepository.DeleteByCartIdAsync(cart.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Order] Failed to clear cart for user {UserId}", userId);
            }
        }
    }
}
