using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.ShoppingCart.Commands.ClearCart;
using E_LaptopShop.Application.Features.ShoppingCart.Commands.CreateShoppingCard;
using E_LaptopShop.Application.Features.ShoppingCart.Commands.RemoveFromCart;
using E_LaptopShop.Application.Features.ShoppingCart.Commands.UpdateCartItem;
using E_LaptopShop.Application.Features.ShoppingCart.Queries.GetCart;
using E_LaptopShop.Application.Features.ShoppingCart.Queries.GetCartSummary;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_LaptopShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 Require JWT authentication for all shopping cart operations
    [Tags("👤 Customer")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingCartController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Lấy giỏ hàng của user hiện tại (bao gồm tất cả items)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ShoppingCartDto>>> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var query = new GetCartQuery { UserId = userId };
                var cart = await _mediator.Send(query);

                return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart, "Cart retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ShoppingCartDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Lấy tổng kết giỏ hàng (tổng tiền, số lượng, discount)
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<CartSummaryDto>>> GetCartSummary()
        {
            try
            {
                var userId = GetUserId();
                var query = new GetCartSummaryQuery { UserId = userId };
                var summary = await _mediator.Send(query);

                return Ok(ApiResponse<CartSummaryDto>.SuccessResponse(summary, "Cart summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CartSummaryDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Lấy số lượng items trong giỏ hàng (dùng cho badge/counter)
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetCartItemCount()
        {
            try
            {
                var userId = GetUserId();
                var query = new GetCartSummaryQuery { UserId = userId };
                var summary = await _mediator.Send(query);

                return Ok(ApiResponse<int>.SuccessResponse(summary.TotalItems, "Cart item count retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<int>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng (nếu đã có thì tăng số lượng)
        /// </summary>
        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<ShoppingCartItemDto>>> AddToCart([FromBody] AddToCartCommand command)
        {
            try
            {
                command.UserId = GetUserId();
                var cartItem = await _mediator.Send(command);

                return Ok(ApiResponse<ShoppingCartItemDto>.SuccessResponse(cartItem, "Item added to cart successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ShoppingCartItemDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ShoppingCartItemDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng (nếu quantity = 0 thì xóa)
        /// </summary>
        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<ApiResponse<ShoppingCartItemDto>>> UpdateCartItem(int itemId, [FromBody] UpdateCartItemCommand command)
        {
            try
            {
                command.ItemId = itemId;
                command.UserId = GetUserId();
                var cartItem = await _mediator.Send(command);

                if (cartItem == null)
                {
                    return Ok(ApiResponse<ShoppingCartItemDto>.SuccessResponse(null, "Item removed from cart"));
                }

                return Ok(ApiResponse<ShoppingCartItemDto>.SuccessResponse(cartItem, "Cart item updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ShoppingCartItemDto>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ShoppingCartItemDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng
        /// </summary>
        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFromCart(int itemId)
        {
            try
            {
                var command = new RemoveFromCartCommand
                {
                    ItemId = itemId,
                    UserId = GetUserId()
                };
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Cart item not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Item removed from cart successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Xóa toàn bộ giỏ hàng (xóa tất cả items)
        /// </summary>
        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
        {
            try
            {
                var command = new ClearCartCommand { UserId = GetUserId() };
                var result = await _mediator.Send(command);

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Cart cleared successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
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
}
