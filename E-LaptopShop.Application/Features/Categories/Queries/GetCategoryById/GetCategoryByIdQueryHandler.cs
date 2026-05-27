using AutoMapper;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _categoryService.GetByIdAsync(request.Id, cancellationToken);
            if (dto is null)
                throw new KeyNotFoundException($"Category with ID {request.Id} not found");
            return dto;
        }
    }
}
