using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; } // For access control - null means admin access
    }
}
