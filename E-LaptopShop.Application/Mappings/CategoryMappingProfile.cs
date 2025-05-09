using AutoMapper;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;
using E_LaptopShop.Application.Features.Categories.Commands.UpdateCategory;

namespace E_LaptopShop.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, Category>();
    }
} 