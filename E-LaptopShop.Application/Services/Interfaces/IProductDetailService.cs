using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Services.Interfaces
{
    /// <summary>
    /// Service phục vụ trang chi tiết sản phẩm — gom dữ liệu sản phẩm,
    /// thông số, bình luận, đánh giá và phân quyền theo người xem.
    /// </summary>
    public interface IProductDetailService
    {
        /// <summary>Chi tiết sản phẩm + thông số + ảnh + thống kê review.</summary>
        Task<ProductDetailDto> GetProductDetailAsync(int productId, CancellationToken ct = default);

        /// <summary>Resolve slug → productId. Throws KeyNotFoundException nếu không có.</summary>
        Task<int> ResolveSlugToIdAsync(string slug, CancellationToken ct = default);

        /// <summary>
        /// Trả về ngữ cảnh của user hiện tại cho 1 sản phẩm:
        /// đã đăng nhập chưa, đã mua sản phẩm này chưa, danh hiệu loyalty.
        /// </summary>
        Task<CurrentUserContextDto> GetCurrentUserContextAsync(int? userId, int productId, CancellationToken ct = default);

        // Comments
        Task<List<ProductCommentDto>> GetCommentsAsync(int productId, CancellationToken ct = default);
        Task<ProductCommentDto> AddCommentAsync(int userId, CreateProductCommentDto dto, CancellationToken ct = default);
        Task<int> DeleteCommentAsync(int currentUserId, string currentUserRole, int commentId, CancellationToken ct = default);

        // Reviews
        Task<List<ProductReviewDto>> GetReviewsAsync(int productId, CancellationToken ct = default);

        /// <summary>
        /// Tạo review. Ném <see cref="System.UnauthorizedAccessException"/>
        /// nếu user chưa mua sản phẩm (chỉ user đã mua mới được đánh giá).
        /// </summary>
        Task<ProductReviewDto> AddReviewAsync(int userId, CreateProductReviewDto dto, CancellationToken ct = default);

        Task<int> DeleteReviewAsync(int currentUserId, string currentUserRole, int reviewId, CancellationToken ct = default);
    }
}
