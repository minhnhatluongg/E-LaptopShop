using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string OrderNumber { get; set; } = null!;

    public int? UserId { get; set; }

    public int? ShippingAddressId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SubTotal { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [StringLength(100)]
    public string? DiscountCode { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ShippingFee { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string? ShippingMethod { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    public bool IsPaid { get; set; } = false;

    [Column(TypeName = "datetime")]
    public DateTime? PaidDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // Navigation properties
    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }

    [ForeignKey("ShippingAddressId")]
    public virtual UserAddress? ShippingAddress { get; set; }
}
