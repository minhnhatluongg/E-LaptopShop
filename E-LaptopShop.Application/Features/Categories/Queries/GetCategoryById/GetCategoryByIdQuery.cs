using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public int Id { get; init; }
    }
}
