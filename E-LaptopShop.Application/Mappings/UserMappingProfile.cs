using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.Auth;
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
                .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}".Trim()))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));
            CreateMap<CreateUserCommand, User>();
            CreateMap<UpdateUserCommand, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RegisterRequestDto, User>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Trim().ToLowerInvariant()))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName.Trim()))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName.Trim()))
            .ForMember(d => d.Phone, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Phone) ? null : s.Phone.Trim()))
            .ForMember(d => d.Gender, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Gender) ? null : s.Gender.Trim()))
            .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateOfBirth))

            // các field hệ thống: KHÔNG map từ client
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.RoleId, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.EmailConfirmed, o => o.Ignore())
            .ForMember(d => d.LoginAttempts, o => o.Ignore())
            .ForMember(d => d.IsLocked, o => o.Ignore())
            .ForMember(d => d.LockedUntil, o => o.Ignore())
            .ForMember(d => d.Token, o => o.Ignore())
            .ForMember(d => d.RefreshToken, o => o.Ignore())
            .ForMember(d => d.RefreshTokenExpiryTime, o => o.Ignore())
            .ForMember(d => d.AvatarUrl, o => o.Ignore())
            .ForMember(d => d.VerificationToken, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.UpdatedBy, o => o.Ignore())

            // Không cần validate ConfirmPassword trong mapping
            .ForSourceMember(s => s.ConfirmPassword, o => o.DoNotValidate());
        }
    }
}
