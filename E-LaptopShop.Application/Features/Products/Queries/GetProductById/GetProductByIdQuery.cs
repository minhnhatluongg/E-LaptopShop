using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; init; }
} 