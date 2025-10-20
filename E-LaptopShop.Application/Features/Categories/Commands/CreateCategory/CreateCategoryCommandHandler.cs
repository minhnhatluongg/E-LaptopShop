using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryService _categoriesService;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    public CreateCategoryCommandHandler(ICategoryService categoryService, ILogger<CreateCategoryCommandHandler> logger)
    {
        _logger = logger;
        _categoriesService = categoryService;
    }
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category : {Name}", request.RequestDto.Name);
        var created = await _categoriesService.CreateAsync(request.RequestDto, cancellationToken);
        return created;
    }
} 