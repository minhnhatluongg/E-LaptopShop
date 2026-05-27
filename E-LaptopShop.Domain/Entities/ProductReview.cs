using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class ProductReview
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public int? UserId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual Product? Product { get; set; }
    public virtual User? User { get; set; }
}
