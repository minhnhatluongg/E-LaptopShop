using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings
{
    public class BannerMappingProfile : Profile
    {
        public BannerMappingProfile()
        {
            CreateMap<Banner, BannerDto>();
            CreateMap<CreateBannerDto, Banner>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());
            CreateMap<UpdateBannerDto, Banner>()
                .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));
        }
    }
}
