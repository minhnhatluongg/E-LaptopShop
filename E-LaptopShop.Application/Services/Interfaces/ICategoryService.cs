using E_LaptopShop.Application.DTOs.QueryParams___forGetAll;
using E_LaptopShop.Application.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface ICategoryService : IBaseService<CategoryDto,CategoryCreateRequestDto,CategoryUpdateRequestDto,CategoriesParams>
    {
    }
}
