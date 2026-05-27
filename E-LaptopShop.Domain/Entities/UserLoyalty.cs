using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class UserLoyalty
{
    public int UserId { get; set; }

    public int TierId { get; set; }

    public int TotalPoints { get; set; }

    public decimal LifetimeSpend { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual LoyaltyTier Tier { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
