using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductTagMap
{
    public int ProductId { get; set; }

    public int TagId { get; set; }

    public virtual ProductTag Tag { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
