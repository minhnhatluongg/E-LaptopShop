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
    public class ShoppingCartItemMappingProfile : Profile
    {
        public ShoppingCartItemMappingProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>()
                 .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity)));

            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                    src.Product.ProductImages.FirstOrDefault(pi => pi.IsMain) != null
                        ? src.Product.ProductImages.FirstOrDefault(pi => pi.IsMain).SysFile.FileUrl
                        : ""))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }
}
