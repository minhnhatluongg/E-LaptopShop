using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Domain.Entities.Category>(request);
        var createdCategory = await _categoryRepository.AddAsync(category, cancellationToken);
        return _mapper.Map<CategoryDto>(createdCategory);
    }
} 