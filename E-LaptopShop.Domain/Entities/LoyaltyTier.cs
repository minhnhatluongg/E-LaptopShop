using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class LoyaltyTier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal MinSpend { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal PointsMultiplier { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<UserLoyalty> UserLoyalty { get; set; } = new List<UserLoyalty>();
}
