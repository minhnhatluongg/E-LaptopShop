using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public string UpdatedBy { get; set; } = string.Empty;
        
        // Additional fields for specific statuses
        public string? TrackingNumber { get; set; } // For Shipped status
        public DateTime? EstimatedDelivery { get; set; } // For Shipped status
        public string? CancelReason { get; set; } // For Cancelled status
    }
}
