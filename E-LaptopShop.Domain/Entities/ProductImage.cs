using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? SysFileId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsMain { get; set; } = false;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UploadedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;

    public string? CreatedBy { get; set; }
    public virtual Product? Product { get; set; }
    public virtual SysFile? SysFile { get; set; }
}
