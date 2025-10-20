using MediatR;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand()
    : IRequest<CategoryDto>
{
    public CategoryCreateRequestDto RequestDto { get; init; } = null!;
}