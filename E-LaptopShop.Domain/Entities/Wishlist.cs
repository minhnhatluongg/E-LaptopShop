using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class Wishlist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public DateTime AddedAt { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
