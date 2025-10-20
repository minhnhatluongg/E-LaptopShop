using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs.QueryParams___forGetAll;
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
        public CategoriesParams QueryParams { get; init; } = new CategoriesParams();
    }
}
