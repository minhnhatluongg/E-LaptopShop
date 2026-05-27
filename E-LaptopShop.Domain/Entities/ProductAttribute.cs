using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductAttribute
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
