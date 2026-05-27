using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// POCO entity — no EF / data-annotation attributes.
    /// All persistence concerns live in
    /// <c>Infra/Data/Configurations/BrandConfiguration.cs</c>.
    /// </summary>
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
