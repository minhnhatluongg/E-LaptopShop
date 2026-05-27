using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class PointTransaction
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public int? OrderId { get; set; }

    public int Points { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Order? Order { get; set; }
}
