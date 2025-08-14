using E_LaptopShop.Application.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        
        public int UserId { get; set; } // User requesting cancellation
        
        [StringLength(500)]
        public string? Reason { get; set; }
        
        public bool IsAdminCancel { get; set; } = false;
    }
}
