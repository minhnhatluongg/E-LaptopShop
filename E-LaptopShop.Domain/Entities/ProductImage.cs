using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductImage
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }
    public int? SysFileId { get; set; }
    [Required]
    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsMain { get; set; } = false;

    [Required]
    [StringLength(50)]
    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public int DisplayOrder { get; set; } = 0;

    [StringLength(255)]
    public string? AltText { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [Column(TypeName = "datetime")]
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product? Product { get; set; }

    [ForeignKey("SysFileId")]
    [InverseProperty("ProductImages")]
    public virtual SysFile? SysFile { get; set; }
}
