using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductSpecification
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? CPU { get; set; }

    public string? RAM { get; set; }

    public string? Storage { get; set; }

    public string? GPU { get; set; }

    public string? Screen { get; set; }

    public string? OS { get; set; }

    public string? Ports { get; set; }

    public string? Weight { get; set; }

    public string? Battery { get; set; }

    public virtual Product? Product { get; set; }
}
