using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// MediatR Command wrapper for UpdateProductRequestDto
/// </summary>
public record UpdateProductCommand : IRequest<ProductDto>
{
    public UpdateProductRequestDto RequestDto { get; init; } = null!;

    public UpdateProductCommand(UpdateProductRequestDto requestDto)
    {
        RequestDto = requestDto;
    }
} 