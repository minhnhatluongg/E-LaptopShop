using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ReturnRequest
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? OrderItemId { get; set; }

    public int? UserId { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime RequestedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolvedBy { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<RefundTransaction> RefundTransactions { get; set; } = new List<RefundTransaction>();

    // Navigation
    public virtual Order Order { get; set; } = null!;
    public virtual OrderItem? OrderItem { get; set; }
    public virtual User? User { get; set; }
}
