using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.User.Commands.CreateUser;
using E_LaptopShop.Application.Features.User.Commands.UpdateUser;
using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));
            CreateMap<CreateUserCommand, User>();
            CreateMap<UpdateUserCommand, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
