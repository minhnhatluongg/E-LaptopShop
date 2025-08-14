using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Orders.Commands.CancelOrder;
using E_LaptopShop.Application.Features.Orders.Commands.CreateOrder;
using E_LaptopShop.Application.Features.Orders.Commands.UpdateOrderStatus;
using E_LaptopShop.Application.Features.Orders.Queries.GetOrderById;
using E_LaptopShop.Application.Features.Orders.Queries.GetOrders;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_LaptopShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all order operations
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo đơn hàng mới từ giỏ hàng hoặc danh sách sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                command.UserId = GetUserId();
                var order = await _mediator.Send(command);

                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của user hiện tại
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetMyOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetOrdersQuery
                {
                    UserId = GetUserId(),
                    Status = status,
                    Page = page,
                    PageSize = pageSize
                };

                var orders = await _mediator.Send(query);
                return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<OrderSummaryDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng theo ID
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int orderId)
        {
            try
            {
                var query = new GetOrderByIdQuery
                {
                    OrderId = orderId,
                    UserId = GetUserId()
                };

                var order = await _mediator.Send(query);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Hủy đơn hàng (chỉ cho đơn hàng ở trạng thái Pending hoặc Confirmed)
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CancelOrder(int orderId, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var command = new CancelOrderCommand
                {
                    OrderId = orderId,
                    UserId = GetUserId(),
                    Reason = request.Reason,
                    IsAdminCancel = false
                };

                var order = await _mediator.Send(command);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order cancelled successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [ADMIN] Lấy tất cả đơn hàng với filter
        /// </summary>
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")] // Require admin role
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetAllOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = new GetOrdersQuery
                {
                    UserId = null, // Admin can see all orders
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SearchTerm = searchTerm,
                    Page = page,
                    PageSize = pageSize
                };

                var orders = await _mediator.Send(query);
                return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<OrderSummaryDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [ADMIN] Cập nhật trạng thái đơn hàng
        /// </summary>
        [HttpPut("admin/{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var command = new UpdateOrderStatusCommand
                {
                    OrderId = orderId,
                    Status = request.Status,
                    Notes = request.Notes,
                    UpdatedBy = GetUserId().ToString(),
                    TrackingNumber = request.TrackingNumber,
                    EstimatedDelivery = request.EstimatedDelivery,
                    CancelReason = request.CancelReason
                };

                var order = await _mediator.Send(command);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order status updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [ADMIN] Hủy đơn hàng (admin có thể hủy ở nhiều trạng thái hơn)
        /// </summary>
        [HttpPost("admin/{orderId}/cancel")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> AdminCancelOrder(int orderId, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var command = new CancelOrderCommand
                {
                    OrderId = orderId,
                    UserId = GetUserId(),
                    Reason = request.Reason,
                    IsAdminCancel = true
                };

                var order = await _mediator.Send(command);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order cancelled successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [ADMIN] Lấy chi tiết đơn hàng (admin có thể xem mọi đơn)
        /// </summary>
        [HttpGet("admin/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> AdminGetOrderById(int orderId)
        {
            try
            {
                var query = new GetOrderByIdQuery
                {
                    OrderId = orderId,
                    UserId = null // Admin can view any order
                };

                var order = await _mediator.Send(query);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse(ex.Message));
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }
    }

    // Request DTOs
    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string? CancelReason { get; set; }
    }
}
