using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class Category
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(150)]
    public string Slug { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    // --- Hierarchy ---
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    // --- Display / Status ---
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    // --- Media (optional) ---
    public long? ImageFileId { get; set; }
    // public SysFile? ImageFile { get; set; } // nếu có bảng file riêng

    // --- SEO (optional) ---
    [StringLength(150)] public string? MetaTitle { get; set; }
    [StringLength(300)] public string? MetaDescription { get; set; }
    [StringLength(200)] public string? MetaKeywords { get; set; }

    // --- Audit ---
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    [StringLength(64)] public string? CreatedBy { get; set; }
    [StringLength(64)] public string? UpdatedBy { get; set; }

    // --- Soft delete ---
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // --- Concurrency ---
    [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // --- Products ---
    [InverseProperty(nameof(Product.Category))]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
