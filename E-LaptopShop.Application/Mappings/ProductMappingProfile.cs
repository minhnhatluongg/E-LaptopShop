using AutoMapper;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Products.Commands.CreateProduct;
using E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

namespace E_LaptopShop.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();
    }
} 