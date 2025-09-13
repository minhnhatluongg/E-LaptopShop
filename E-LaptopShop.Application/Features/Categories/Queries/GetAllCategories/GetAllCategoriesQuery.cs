using E_LaptopShop.Application.Common.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : BasePagedQuery<CategoryDto>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
