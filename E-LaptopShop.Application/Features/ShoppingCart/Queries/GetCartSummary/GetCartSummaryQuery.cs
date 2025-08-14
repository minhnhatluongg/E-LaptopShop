using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ShoppingCart.Queries.GetCartSummary
{
    public class GetCartSummaryQuery : IRequest<CartSummaryDto>
    {
        public int UserId { get; set; }
    }
}
