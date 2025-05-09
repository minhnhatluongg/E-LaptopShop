using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
} 