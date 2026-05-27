using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class AuditLog
{
    public long Id { get; set; }

    public int? UserId { get; set; }

    public string EntityName { get; set; } = null!;

    public string? EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}
