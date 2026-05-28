using System;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateCouponRequestDto
    {
        [Required(ErrorMessage = "Mã coupon là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Mã coupon không được vượt quá 50 ký tự.")]
        public string Code { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc.")]
        [RegularExpression("^(percent|fixed)$", ErrorMessage = "DiscountType chỉ chấp nhận 'percent' hoặc 'fixed'.")]
        public string DiscountType { get; set; } = "percent";

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải > 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinOrderAmount không hợp lệ.")]
        public decimal MinOrderAmount { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; }

        public DateTime StartsAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndsAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCouponRequestDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [RegularExpression("^(percent|fixed)$")]
        public string? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>Trả về khi customer apply coupon — bao gồm số tiền discount thực tế.</summary>
    public class ApplyCouponResultDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = null!;
        public decimal OrderAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Message { get; set; } = "Áp dụng mã thành công";
    }
}
