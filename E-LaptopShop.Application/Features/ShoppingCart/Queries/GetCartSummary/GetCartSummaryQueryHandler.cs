using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Queries.GetCartSummary
{
    public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
    {
        private readonly IShoppingCartRepository _cartRepository;

        public GetCartSummaryQueryHandler(IShoppingCartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(request.UserId, cancellationToken);
            
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return new CartSummaryDto
                {
                    TotalItems = 0,
                    SubTotal = 0,
                    TotalDiscount = 0,
                    TotalAmount = 0,
                    IsEmpty = true
                };
            }

            var totalItems = cart.Items.Sum(x => x.Quantity);
            var subTotal = cart.Items.Sum(x => x.Quantity * x.UnitPrice);
            var totalDiscount = cart.Items.Sum(x => x.Quantity * x.UnitPrice * (decimal)(x.Product?.Discount ?? 0) / 100);
            var totalAmount = subTotal - totalDiscount;

            return new CartSummaryDto
            {
                TotalItems = totalItems,
                SubTotal = subTotal,
                TotalDiscount = totalDiscount,
                TotalAmount = totalAmount,
                IsEmpty = false
            };
        }
    }
}
