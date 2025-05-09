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

    public int? ProductId { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool? IsMain { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product? Product { get; set; }
}
