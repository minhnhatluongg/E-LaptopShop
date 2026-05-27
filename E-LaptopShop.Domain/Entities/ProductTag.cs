using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<ProductTagMap> ProductTagMap { get; set; } = new List<ProductTagMap>();
}
