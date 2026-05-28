using System.Security.Claims;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Coupon.Commands.ApplyCoupon;
using E_LaptopShop.Application.Features.Coupon.Commands.CreateCoupon;
using E_LaptopShop.Application.Features.Coupon.Commands.DeleteCoupon;
using E_LaptopShop.Application.Features.Coupon.Commands.UpdateCoupon;
using E_LaptopShop.Application.Features.Coupon.Queries.GetAllCoupons;
using E_LaptopShop.Application.Features.Coupon.Queries.GetCouponByCode;
using E_LaptopShop.Application.Features.Coupon.Queries.GetCouponById;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1/coupons")]
    public class CouponsController : ApiV1ControllerBase
    {
        private readonly IMediator _mediator;
        public const string EntityName = "Coupon";

        public CouponsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 👑 [ADMIN] Lấy danh sách coupon với phân trang/lọc
        /// </summary>
        [HttpGet]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CouponDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResult<CouponDto>>>> GetAll(
            [FromQuery] CouponQueryParams queryParams)
        {
            var result = await _mediator.Send(new GetAllCouponsQuery { QueryParams = queryParams });
            return Ok(ApiResponse<PagedResult<CouponDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// 🔓 [PUBLIC] Lấy coupon theo ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<CouponDto>>> GetById(int id)
        {
            var coupon = await _mediator.Send(new GetCouponByIdQuery { Id = id });
            if (coupon == null) return NotFound(ApiResponse<CouponDto>.ErrorResponse($"{EntityName} không tồn tại"));
            return Ok(ApiResponse<CouponDto>.SuccessResponse(coupon));
        }

        /// <summary>
        /// 🔓 [PUBLIC] Lấy coupon theo code (dùng cho UI tra mã)
        /// </summary>
        [HttpGet("code/{code}")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<CouponDto>>> GetByCode(string code)
        {
            var coupon = await _mediator.Send(new GetCouponByCodeQuery { Code = code });
            if (coupon == null) return NotFound(ApiResponse<CouponDto>.ErrorResponse($"Mã '{code}' không tồn tại"));
            return Ok(ApiResponse<CouponDto>.SuccessResponse(coupon));
        }

        /// <summary>
        /// 👤 [CUSTOMER] Validate coupon cho đơn hàng — KHÔNG consume.
        /// Dùng ở bước checkout preview để hiển thị số tiền được giảm.
        /// </summary>
        [HttpPost("apply")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<ApplyCouponResultDto>>> Apply(
            [FromBody] ApplyCouponRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<ApplyCouponResultDto>.ErrorResponse("Invalid user"));

            var result = await _mediator.Send(new ApplyCouponCommand
            {
                Code = request.Code,
                OrderAmount = request.OrderAmount,
                UserId = userId,
            });
            return Ok(ApiResponse<ApplyCouponResultDto>.SuccessResponse(result));
        }

        /// <summary>
        /// 👑 [ADMIN] Tạo coupon mới
        /// </summary>
        [HttpPost]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 201)]
        public async Task<ActionResult<ApiResponse<CouponDto>>> Create(
            [FromBody] CreateCouponRequestDto requestDto)
        {
            var created = await _mediator.Send(new CreateCouponCommand { RequestDto = requestDto });
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<CouponDto>.SuccessResponse(created, $"{EntityName} created successfully"));
        }

        /// <summary>
        /// 👑 [ADMIN] Cập nhật coupon (partial — chỉ field gửi lên mới đổi)
        /// </summary>
        [HttpPut("{id:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<CouponDto>>> Update(
            int id, [FromBody] UpdateCouponRequestDto requestDto)
        {
            if (id != requestDto.Id)
                return BadRequest(ApiResponse<CouponDto>.ErrorResponse("ID mismatch between route and body"));

            var updated = await _mediator.Send(new UpdateCouponCommand { Id = id, RequestDto = requestDto });
            return Ok(ApiResponse<CouponDto>.SuccessResponse(updated, $"{EntityName} updated successfully"));
        }

        /// <summary>
        /// 👑 [ADMIN] Xoá coupon
        /// </summary>
        [HttpDelete("{id:int}")]
        [AdminOnly]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeleteCouponCommand { Id = id });
            return Ok(new ApiResponse<int>
            {
                Success = deleted,
                Message = deleted ? $"{EntityName} deleted successfully" : "Không tìm thấy coupon",
                Data = id,
            });
        }
    }

    public class ApplyCouponRequest
    {
        public string Code { get; set; } = null!;
        public decimal OrderAmount { get; set; }
    }
}
