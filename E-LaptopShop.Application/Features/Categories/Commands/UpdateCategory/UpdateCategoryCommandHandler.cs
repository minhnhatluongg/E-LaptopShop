using AutoMapper;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
            }
            _mapper.Map(request, existingCategory);
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }
    }
}
