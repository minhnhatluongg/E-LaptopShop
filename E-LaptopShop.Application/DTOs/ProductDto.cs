using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_LaptopShop.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public int? InStock { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsActive { get; set; }
}
public class CreateProductRequestDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? BrandId { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public decimal? Discount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
    public int InStock { get; set; } = 0;

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
public class UpdateProductRequestDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = null!;
    public int? BrandId { get; set; }
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public decimal? Discount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
    public int InStock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}