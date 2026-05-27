using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// POCO entity — no EF attributes. See
    /// <c>Infra/Data/Configurations/CategoryConfiguration.cs</c>.
    /// </summary>
    public partial class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        // Hierarchy
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        // Display / Status
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        // Media
        public long? ImageFileId { get; set; }

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Concurrency token
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Products belonging to this category
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
