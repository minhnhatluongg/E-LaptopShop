using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            // Lấy order
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
            }

            // Kiểm tra quyền hủy đơn
            if (!request.IsAdminCancel && order.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You don't have permission to cancel this order");
            }

            // Kiểm tra có thể hủy đơn không
            var canCancel = await _orderRepository.CanCancelAsync(request.OrderId, cancellationToken);
            if (!canCancel)
            {
                var currentStatus = Enum.Parse<OrderStatus>(order.Status);
                throw new InvalidOperationException($"Cannot cancel order with status {currentStatus}");
            }

            // Cập nhật status và lý do hủy
            order.Status = OrderStatus.Cancelled.ToString();
            order.Notes = string.IsNullOrEmpty(order.Notes) 
                ? $"Cancelled: {request.Reason}" 
                : $"{order.Notes}\nCancelled: {request.Reason}";
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = request.UserId.ToString();

            // Lưu thay đổi
            await _orderRepository.UpdateAsync(order, cancellationToken);

            // TODO: Thực hiện các logic khác khi hủy đơn
            // - Hoàn lại inventory
            // - Xử lý refund nếu đã thanh toán
            // - Gửi notification

            // Load lại order với đầy đủ thông tin
            var updatedOrder = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
    }
}
