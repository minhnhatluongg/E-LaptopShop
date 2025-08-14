using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderSummaryDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderSummaryDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Order> orders;

            if (request.UserId.HasValue)
            {
                // Get orders for specific user
                orders = await _orderRepository.GetByUserIdAsync(request.UserId.Value, request.Page, request.PageSize, cancellationToken);
            }
            else if (request.Status.HasValue)
            {
                // Get orders by status (admin view)
                orders = await _orderRepository.GetByStatusAsync(request.Status.Value, request.Page, request.PageSize, cancellationToken);
            }
            else
            {
                // Get all orders (admin view)
                orders = await _orderRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
            }

            // TODO: Apply additional filters (date range, search term)
            // TODO: Implement filtering logic

            return _mapper.Map<IEnumerable<OrderSummaryDto>>(orders);
        }
    }
}
