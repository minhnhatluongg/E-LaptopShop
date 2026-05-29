using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs
{
    /// <summary>
    /// DTO tổng hợp dữ liệu trang chi tiết sản phẩm:
    /// thông tin sản phẩm + ảnh + thông số kỹ thuật + thống kê đánh giá.
    /// </summary>
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int? InStock { get; set; }
        public bool IsActive { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }

        public List<ProductImageDto> Images { get; set; } = new();
        public ProductSpecificationDto? Specification { get; set; }

        // Thống kê review
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalComments { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new();
    }

    /// <summary>Đánh giá (rating) — chỉ user đã mua sản phẩm này mới được tạo.</summary>
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserAvatarUrl { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }

        /// <summary>True nếu user này đã mua chính sản phẩm được review.</summary>
        public bool IsVerifiedPurchase { get; set; }

        /// <summary>Tên danh hiệu (loyalty tier) của user — vd: "Đồng", "Bạc", "Vàng".</summary>
        public string? LoyaltyTierName { get; set; }
    }

    public class CreateProductReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }
    }

    /// <summary>Bình luận — user đã đăng nhập là được, không yêu cầu mua hàng.</summary>
    public class ProductCommentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserAvatarUrl { get; set; }
        public string Content { get; set; } = null!;
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>True nếu user này đã mua chính sản phẩm đang xem.</summary>
        public bool HasPurchasedThisProduct { get; set; }

        /// <summary>Tên danh hiệu (loyalty tier) của user comment.</summary>
        public string? LoyaltyTierName { get; set; }
    }

    public class CreateProductCommentDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Content { get; set; } = null!;

        public int? ParentCommentId { get; set; }
    }

    /// <summary>Thông tin quyền hạn của user hiện tại trên trang chi tiết.</summary>
    public class CurrentUserContextDto
    {
        public bool IsAuthenticated { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Role { get; set; }
        public string? LoyaltyTierName { get; set; }

        /// <summary>User đã mua chính sản phẩm đang xem chưa?</summary>
        public bool HasPurchasedThisProduct { get; set; }

        public bool CanComment => IsAuthenticated;
        public bool CanReview => IsAuthenticated && HasPurchasedThisProduct;
    }
}
