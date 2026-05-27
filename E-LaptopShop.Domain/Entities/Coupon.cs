using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class Coupon
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

    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
}
