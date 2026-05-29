using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;

namespace E_LaptopShop.Application.Features.Orders.Commands.CreateGuestOrder
{
    public class CreateGuestOrderCommandHandler : IRequestHandler<CreateGuestOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public CreateGuestOrderCommandHandler(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(CreateGuestOrderCommand request, CancellationToken ct)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Giỏ hàng trống");

            // Tính subtotal từ giá hiện tại
            decimal subTotal = 0m;
            var items = new System.Collections.Generic.List<OrderItem>();
            foreach (var it in request.Items)
            {
                var p = await _productRepo.GetByIdAsync(it.ProductId, ct);
                if (p == null) throw new KeyNotFoundException($"Sản phẩm {it.ProductId} không tồn tại");

                var unitPrice = p.Discount.HasValue && p.Discount > 0
                    ? Math.Round(p.Price * (1 - p.Discount.Value / 100m), 0)
                    : p.Price;
                var lineSubTotal = unitPrice * it.Quantity;

                items.Add(new OrderItem
                {
                    ProductId       = p.Id,
                    Quantity        = it.Quantity,
                    UnitPrice       = unitPrice,
                    DiscountPercent = p.Discount ?? 0,
                    SubTotal        = lineSubTotal,
                    Status          = "Pending",
                });
                subTotal += lineSubTotal;
            }

            // Lưu thông tin guest vào Notes dưới dạng JSON (để giữ schema, chưa cần migration)
            var guestInfo = JsonSerializer.Serialize(new
            {
                request.FullName,
                request.Phone,
                request.Email,
                request.Address,
                request.Ward,
                request.District,
                request.City,
            });

            var order = new Order
            {
                OrderNumber    = $"GUEST-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}",
                UserId         = null,
                OrderDate      = DateTime.UtcNow,
                Status         = "Pending",
                SubTotal       = subTotal,
                DiscountAmount = 0,
                DiscountCode   = request.DiscountCode,
                TaxAmount      = 0,
                ShippingFee    = 0,
                TotalAmount    = subTotal,
                ShippingMethod = request.ShippingMethod,
                PaymentMethod  = request.PaymentMethod,
                IsPaid         = false,
                Notes          = $"GUEST_INFO={guestInfo}" +
                                 (string.IsNullOrWhiteSpace(request.Notes) ? "" : $" | {request.Notes}"),
                CreatedAt      = DateTime.UtcNow,
                CreatedBy      = "guest",
            };

            foreach (var i in items) order.OrderItems.Add(i);

            var created = await _orderRepo.CreateAsync(order, ct);
            var full    = await _orderRepo.GetByIdAsync(created.Id, ct);
            return _mapper.Map<OrderDto>(full);
        }
    }
}
