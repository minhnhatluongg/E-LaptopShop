using AutoMapper.Configuration.Annotations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public int Id { get; set; }  
        public CategoryUpdateRequestDto RequestDto { get; init; } = null!;
    }
}
