using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class RefundTransaction
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? PaymentTransactionId { get; set; }

    public int? ReturnRequestId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? RefundMethod { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ReturnRequest? ReturnRequest { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual PaymentTransaction? PaymentTransaction { get; set; }
}
