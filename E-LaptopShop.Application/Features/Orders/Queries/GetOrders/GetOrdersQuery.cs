using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace E_LaptopShop.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQuery : IRequest<IEnumerable<OrderSummaryDto>>
    {
        public int? UserId { get; set; } // Null = get all orders (for admin)
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } // Search by order number or customer name
        public bool IncludeItems { get; set; } = false;
    }
}
