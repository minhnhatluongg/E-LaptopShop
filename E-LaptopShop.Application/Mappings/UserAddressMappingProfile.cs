using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.UserAddress.Commands.CreateUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Commands.UpdateUserAddress;
using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Mappings
{
    public class UserAddressMappingProfile : Profile
    {
        public UserAddressMappingProfile()
        {
            //1.Create Commands -> Entity
            CreateMap<CreateUserAddressCommand, UserAddress>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId))
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(s => false))
                .ForMember(d => d.DeletedAt, opt => opt.MapFrom(s => (DateTimeOffset?)null))
                .ForMember(d => d.IsDefault, opt => opt.MapFrom(s => s.IsDefault))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateUserAddressCommand, UserAddress>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.DeletedAt, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                // UpdatedAt luôn cập nhật khi update
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.IsDefault, o => o.Ignore());

            CreateMap<UserAddress, UserAddressDto>()
                .ForMember(d => d.IsDefault, o => o.MapFrom(s => s.IsDefault));
        }
    }
}
