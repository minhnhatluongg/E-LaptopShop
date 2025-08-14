using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
            }

            // Check access permission
            if (request.UserId.HasValue && order.UserId != request.UserId.Value)
            {
                throw new UnauthorizedAccessException("You don't have permission to view this order");
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
