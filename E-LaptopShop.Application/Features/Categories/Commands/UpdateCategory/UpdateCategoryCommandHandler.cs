using AutoMapper;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommandHandler> _log;

        public UpdateCategoryCommandHandler(
            ICategoryService categoryService,
            IMapper mapper,
            ILogger<UpdateCategoryCommandHandler> log
            )
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _log = log;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            _log.LogInformation("Updating Category {Id} ", request.Id);
            var dto = request.RequestDto;
            dto.Id = request.Id;
            var result = await _categoryService.UpdateAsync(request.Id, dto, cancellationToken);
            return result;
        }
    }
}
