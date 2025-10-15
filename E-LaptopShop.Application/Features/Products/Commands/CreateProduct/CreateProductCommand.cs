using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductDto>
{
    public CreateProductRequestDto RequestDto { get; init; } = null!;

    public CreateProductCommand(CreateProductRequestDto requestDto)
    {
        RequestDto = requestDto;
    }
} 