using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductSpecification
{
    [Key]
    public int Id { get; set; }

    public int? ProductId { get; set; }

    [StringLength(100)]
    public string? CPU { get; set; }

    [StringLength(50)]
    public string? RAM { get; set; }

    [StringLength(100)]
    public string? Storage { get; set; }

    [StringLength(100)]
    public string? GPU { get; set; }

    [StringLength(100)]
    public string? Screen { get; set; }

    [StringLength(50)]
    public string? OS { get; set; }

    [StringLength(255)]
    public string? Ports { get; set; }

    [StringLength(50)]
    public string? Weight { get; set; }

    [StringLength(50)]
    public string? Battery { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductSpecifications")]
    public virtual Product? Product { get; set; }
}
