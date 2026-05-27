using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductAttributeValue
{
    public int Id { get; set; }

    public int AttributeId { get; set; }

    public string Value { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public virtual ProductAttribute Attribute { get; set; } = null!;

    public virtual ICollection<ProductVariant> Variant { get; set; } = new List<ProductVariant>();
}
