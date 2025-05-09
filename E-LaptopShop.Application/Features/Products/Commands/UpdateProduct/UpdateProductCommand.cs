using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<ProductDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? Discount { get; init; }
    public int? InStock { get; init; }
    public int? CategoryId { get; init; }
} 