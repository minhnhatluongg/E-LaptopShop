using System;

namespace E_LaptopShop.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public int? InStock { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public int? InStock { get; set; }
    public int? CategoryId { get; set; }
}

public class UpdateProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? Discount { get; set; }
    public int? InStock { get; set; }
    public int? CategoryId { get; set; }
} 