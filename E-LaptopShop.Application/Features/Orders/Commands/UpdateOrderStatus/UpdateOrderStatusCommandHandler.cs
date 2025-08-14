using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Lấy order hiện tại
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found");
            }

            var currentStatus = Enum.Parse<OrderStatus>(order.Status);
            
            // Validate status transition
            ValidateStatusTransition(currentStatus, request.Status);

            // Cập nhật order status
            order = await _orderRepository.UpdateStatusAsync(request.OrderId, request.Status, request.UpdatedBy, cancellationToken);

            // Cập nhật additional fields dựa trên status
            await UpdateAdditionalFields(order, request, cancellationToken);

            // Cập nhật status của order items
            await UpdateOrderItemsStatus(request.OrderId, request.Status, cancellationToken);

            // Xử lý logic nghiệp vụ theo status
            await HandleStatusSpecificLogic(order, request, cancellationToken);

            // Load lại order với đầy đủ thông tin
            var updatedOrder = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            return _mapper.Map<OrderDto>(updatedOrder);
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid status transitions
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                [OrderStatus.Pending] = new() { OrderStatus.Confirmed, OrderStatus.Cancelled },
                [OrderStatus.Confirmed] = new() { OrderStatus.Processing, OrderStatus.Cancelled },
                [OrderStatus.Processing] = new() { OrderStatus.Shipped, OrderStatus.Cancelled },
                [OrderStatus.Shipped] = new() { OrderStatus.Delivered },
                [OrderStatus.Delivered] = new() { OrderStatus.Completed, OrderStatus.Returned },
                [OrderStatus.Completed] = new() { OrderStatus.Returned },
                [OrderStatus.Cancelled] = new() { }, // No transitions from cancelled
                [OrderStatus.Returned] = new() { OrderStatus.Refunded },
                [OrderStatus.Refunded] = new() { } // Final state
            };

            if (!validTransitions.ContainsKey(currentStatus) || 
                !validTransitions[currentStatus].Contains(newStatus))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from {currentStatus} to {newStatus}");
            }
        }

        private async Task UpdateAdditionalFields(Domain.Entities.Order order, UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            switch (request.Status)
            {
                case OrderStatus.Shipped:
                    // TODO: Update tracking number and estimated delivery
                    break;
                case OrderStatus.Delivered:
                    // TODO: Update delivery date
                    break;
                case OrderStatus.Cancelled:
                    // TODO: Update cancel reason
                    break;
            }

            if (!string.IsNullOrEmpty(request.Notes))
            {
                order.Notes = request.Notes;
                await _orderRepository.UpdateAsync(order, cancellationToken);
            }
        }

        private async Task UpdateOrderItemsStatus(int orderId, OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);
            
            var itemStatus = MapOrderStatusToItemStatus(orderStatus);
            
            foreach (var item in orderItems)
            {
                await _orderItemRepository.UpdateStatusAsync(item.Id, itemStatus, cancellationToken);
            }
        }

        private OrderItemStatus MapOrderStatusToItemStatus(OrderStatus orderStatus)
        {
            return orderStatus switch
            {
                OrderStatus.Pending => OrderItemStatus.Pending,
                OrderStatus.Confirmed => OrderItemStatus.Confirmed,
                OrderStatus.Processing => OrderItemStatus.Confirmed,
                OrderStatus.Shipped => OrderItemStatus.Shipped,
                OrderStatus.Delivered => OrderItemStatus.Delivered,
                OrderStatus.Completed => OrderItemStatus.Delivered,
                OrderStatus.Returned => OrderItemStatus.Returned,
                _ => OrderItemStatus.Pending
            };
        }

        private async Task HandleStatusSpecificLogic(Domain.Entities.Order order, UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            switch (request.Status)
            {
                case OrderStatus.Confirmed:
                    await HandleOrderConfirmed(order, cancellationToken);
                    break;
                case OrderStatus.Cancelled:
                    await HandleOrderCancelled(order, cancellationToken);
                    break;
                case OrderStatus.Delivered:
                    await HandleOrderDelivered(order, cancellationToken);
                    break;
            }
        }

        private async Task HandleOrderConfirmed(Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            // TODO: 
            // - Trừ inventory
            // - Gửi email xác nhận
            // - Tạo invoice
        }

        private async Task HandleOrderCancelled(Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            // TODO:
            // - Hoàn lại inventory
            // - Xử lý refund nếu đã thanh toán
            // - Gửi email thông báo hủy
        }

        private async Task HandleOrderDelivered(Domain.Entities.Order order, CancellationToken cancellationToken)
        {
            // TODO:
            // - Gửi email delivered
            // - Cập nhật loyalty points
            // - Tạo notification
        }
    }
}
