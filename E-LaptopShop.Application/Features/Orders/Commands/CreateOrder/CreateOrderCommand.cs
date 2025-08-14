using E_LaptopShop.Application.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public int UserId { get; set; }
        
        [Required]
        public int ShippingAddressId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? ShippingMethod { get; set; }
        
        [StringLength(100)]
        public string? DiscountCode { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        // Option: Tạo từ giỏ hàng hoặc từ danh sách items
        public bool CreateFromCart { get; set; } = true;
        
        // Nếu không tạo từ cart, sử dụng list items này
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
    
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public decimal? CustomPrice { get; set; } // Cho admin tạo order với giá custom
    }
}
