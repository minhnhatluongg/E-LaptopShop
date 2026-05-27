using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class Notification
{
    public long Id { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string Type { get; set; } = null!;

    public string? Url { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}
