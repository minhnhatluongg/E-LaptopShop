using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string SKU { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }

    public string? Barcode { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ProductAttributeValue> AttributeValue { get; set; } = new List<ProductAttributeValue>();

    // Navigation
    public virtual Product Product { get; set; } = null!;
}
