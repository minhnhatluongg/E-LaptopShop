using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Mappings
{
    public class ProductImageMappingProfile : Profile
    {
        public ProductImageMappingProfile() 
        {
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<CreateProductImageDto, ProductImage>();
            CreateMap<UpdateProductImageDto, ProductImage>();
        }
    }
}
