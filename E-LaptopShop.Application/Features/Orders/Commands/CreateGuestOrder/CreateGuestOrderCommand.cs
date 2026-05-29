using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Orders.Commands.CreateOrder;
using MediatR;

namespace E_LaptopShop.Application.Features.Orders.Commands.CreateGuestOrder
{
    /// <summary>
    /// Tạo đơn hàng cho KHÁCH (chưa đăng nhập).
    /// Order.UserId = null. Thông tin liên hệ lưu vào Notes (JSON).
    /// Frontend gửi guest_cart từ localStorage qua field Items.
    /// </summary>
    public class CreateGuestOrderCommand : IRequest<OrderDto>
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress, StringLength(150)]
        public string? Email { get; set; }

        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ward { get; set; }
        [StringLength(100)]
        public string? District { get; set; }
        [StringLength(100)]
        public string? City { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; } = "COD"; // mặc định COD

        [StringLength(50)]
        public string? ShippingMethod { get; set; }

        [StringLength(100)]
        public string? DiscountCode { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required, MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
