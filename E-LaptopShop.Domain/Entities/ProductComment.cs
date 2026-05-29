using System;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// Bình luận của người dùng đã đăng nhập trên trang chi tiết sản phẩm.
    /// Khác với ProductReview (đánh giá có sao), ProductComment chỉ là nội dung text.
    /// POCO entity — see Infra/Data/Configurations/ProductCommentConfiguration.cs.
    /// </summary>
    public partial class ProductComment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public int? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }
        public virtual ProductComment? ParentComment { get; set; }
    }
}
