using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class PaymentTransaction
{
    public int Id { get; set; }
    public int? OrderId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public DateTime? TransactionDate { get; set; }
    public virtual Order? Order { get; set; }
    public virtual ICollection<RefundTransaction> RefundTransactions { get; set; } = new List<RefundTransaction>();
}
