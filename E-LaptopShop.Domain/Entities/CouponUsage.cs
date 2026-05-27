using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class CouponUsage
{
    public long Id { get; set; }

    public int CouponId { get; set; }

    public int? UserId { get; set; }

    public int? OrderId { get; set; }

    public decimal AmountSaved { get; set; }

    public DateTime UsedAt { get; set; }

    public virtual Coupon Coupon { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual Order? Order { get; set; }
}
