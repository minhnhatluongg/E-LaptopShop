using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ShoppingCart.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, ShoppingCartDto>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IMapper _mapper;

        public GetCartQueryHandler(
            IShoppingCartRepository cartRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }
        public async Task<ShoppingCartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(request.UserId, cancellationToken);
            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}
