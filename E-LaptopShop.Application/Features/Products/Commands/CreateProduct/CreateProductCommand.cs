using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? Discount { get; init; }
    public int? InStock { get; init; }
    public int? CategoryId { get; init; }
} 