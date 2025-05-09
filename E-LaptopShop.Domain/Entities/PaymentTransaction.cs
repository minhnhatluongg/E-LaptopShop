using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class PaymentTransaction
{
    [Key]
    public int Id { get; set; }

    public int? OrderId { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TransactionDate { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("PaymentTransactions")]
    public virtual Order? Order { get; set; }
}
