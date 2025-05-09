using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Roles.Command.CreateRole;
using E_LaptopShop.Application.Features.Roles.Command.UpdateRole;
using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<CreateRoleCommand, Role>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)); 
            CreateMap<UpdateRoleCommand, Role>();
        }
    }
}
