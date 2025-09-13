using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.CreateProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.UpdateProductSpecification;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings;

public class ProductSpecificationMappingProfile : Profile
{
    public ProductSpecificationMappingProfile()
    {
        CreateMap<ProductSpecification, ProductSpecificationDto>();
        CreateMap<CreateProductSpecificationCommand, ProductSpecification>();
        CreateMap<UpdateProductSpecificationCommand, ProductSpecification>();
    }
} 