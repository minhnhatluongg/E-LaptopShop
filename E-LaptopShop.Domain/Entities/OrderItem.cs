using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal DiscountPercent { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal SubTotal { get; set; }
    public string? SKU { get; set; }
    public string? SerialNumber { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
}
