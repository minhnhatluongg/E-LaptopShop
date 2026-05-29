using System.Security.Claims;
using System.Threading.Tasks;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    /// <summary>
    /// API trang chi tiết sản phẩm: thông số, comment, review, phân quyền.
    /// - Comment: yêu cầu đăng nhập.
    /// - Review: chỉ user đã MUA chính sản phẩm này (status Delivered/Completed) mới được tạo.
    /// </summary>
    public class ProductDetailController : ApiV1ControllerBase
    {
        private readonly IProductDetailService _service;

        public ProductDetailController(IProductDetailService service)
        {
            _service = service;
        }

        private int? GetUserIdOrNull()
        {
            var raw = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) && id > 0 ? id : (int?)null;
        }

        private string GetUserRole()
            => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        // -------------------- PRODUCT DETAIL --------------------

        /// <summary>🔓 [PUBLIC] Lấy chi tiết sản phẩm: thông số, ảnh, thống kê review.</summary>
        [HttpGet("{productId:int}")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetDetail(int productId)
        {
            var data = await _service.GetProductDetailAsync(productId);
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(data));
        }

        /// <summary>
        /// 🔓 [PUBLIC] Lấy chi tiết sản phẩm theo SLUG (URL-friendly).
        /// Frontend gọi endpoint này khi route dạng /products/{slug}.
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetDetailBySlug(string slug)
        {
            var id = await _service.ResolveSlugToIdAsync(slug);
            var data = await _service.GetProductDetailAsync(id);
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(data));
        }

        /// <summary>
        /// 🔓 [PUBLIC] Ngữ cảnh người xem trên sản phẩm này:
        /// IsAuthenticated, HasPurchasedThisProduct, LoyaltyTierName, CanComment, CanReview.
        /// Frontend dùng để biết có cho comment/review hay không.
        /// </summary>
        [HttpGet("{productId:int}/context")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<CurrentUserContextDto>>> GetContext(int productId)
        {
            var data = await _service.GetCurrentUserContextAsync(GetUserIdOrNull(), productId);
            return Ok(ApiResponse<CurrentUserContextDto>.SuccessResponse(data));
        }

        /// <summary>🔓 [PUBLIC] Context theo slug.</summary>
        [HttpGet("by-slug/{slug}/context")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<CurrentUserContextDto>>> GetContextBySlug(string slug)
        {
            var id = await _service.ResolveSlugToIdAsync(slug);
            var data = await _service.GetCurrentUserContextAsync(GetUserIdOrNull(), id);
            return Ok(ApiResponse<CurrentUserContextDto>.SuccessResponse(data));
        }

        // -------------------- COMMENTS --------------------

        /// <summary>🔓 [PUBLIC] Danh sách bình luận. Mỗi comment kèm HasPurchasedThisProduct + LoyaltyTierName.</summary>
        [HttpGet("{productId:int}/comments")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<object>>> GetComments(int productId)
        {
            var data = await _service.GetCommentsAsync(productId);
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        /// <summary>🔓 [PUBLIC] Bình luận theo slug.</summary>
        [HttpGet("by-slug/{slug}/comments")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<object>>> GetCommentsBySlug(string slug)
        {
            var id = await _service.ResolveSlugToIdAsync(slug);
            var data = await _service.GetCommentsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        /// <summary>👤 [CUSTOMER] Đăng bình luận — yêu cầu đăng nhập.</summary>
        [HttpPost("comments")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<ProductCommentDto>>> AddComment([FromBody] CreateProductCommentDto dto)
        {
            var userId = GetUserIdOrNull();
            if (userId == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Vui lòng đăng nhập"));

            var created = await _service.AddCommentAsync(userId.Value, dto);
            return Ok(ApiResponse<ProductCommentDto>.SuccessResponse(created, "Đã đăng bình luận"));
        }

        /// <summary>👤/👑 Xóa bình luận — chỉ chủ sở hữu hoặc Admin/Manager.</summary>
        [HttpDelete("comments/{commentId:int}")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<int>>> DeleteComment(int commentId)
        {
            var userId = GetUserIdOrNull();
            if (userId == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Vui lòng đăng nhập"));
            try
            {
                var id = await _service.DeleteCommentAsync(userId.Value, GetUserRole(), commentId);
                return Ok(ApiResponse<int>.SuccessResponse(id, "Đã xóa bình luận"));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        // -------------------- REVIEWS --------------------

        /// <summary>🔓 [PUBLIC] Danh sách đánh giá. Kèm IsVerifiedPurchase + LoyaltyTierName.</summary>
        [HttpGet("{productId:int}/reviews")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<object>>> GetReviews(int productId)
        {
            var data = await _service.GetReviewsAsync(productId);
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        /// <summary>🔓 [PUBLIC] Đánh giá theo slug.</summary>
        [HttpGet("by-slug/{slug}/reviews")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<object>>> GetReviewsBySlug(string slug)
        {
            var id = await _service.ResolveSlugToIdAsync(slug);
            var data = await _service.GetReviewsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        /// <summary>
        /// 👤 [CUSTOMER] Đánh giá sản phẩm — chỉ user đã MUA chính sản phẩm này
        /// (Order status Delivered hoặc Completed) mới được tạo.
        /// </summary>
        [HttpPost("reviews")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<ProductReviewDto>>> AddReview([FromBody] CreateProductReviewDto dto)
        {
            var userId = GetUserIdOrNull();
            if (userId == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Vui lòng đăng nhập"));
            try
            {
                var created = await _service.AddReviewAsync(userId.Value, dto);
                return Ok(ApiResponse<ProductReviewDto>.SuccessResponse(created, "Đã gửi đánh giá"));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👤/👑 Xóa đánh giá — chỉ chủ sở hữu hoặc Admin/Manager.</summary>
        [HttpDelete("reviews/{reviewId:int}")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<int>>> DeleteReview(int reviewId)
        {
            var userId = GetUserIdOrNull();
            if (userId == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Vui lòng đăng nhập"));
            try
            {
                var id = await _service.DeleteReviewAsync(userId.Value, GetUserRole(), reviewId);
                return Ok(ApiResponse<int>.SuccessResponse(id, "Đã xóa đánh giá"));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
