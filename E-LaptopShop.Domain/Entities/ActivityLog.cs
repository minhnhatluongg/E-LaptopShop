using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ActivityLog
{
    public long Id { get; set; }

    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public string EventType { get; set; } = null!;

    public string? Metadata { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}
