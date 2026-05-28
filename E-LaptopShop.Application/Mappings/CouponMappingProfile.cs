using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<Coupon, CouponDto>();
            CreateMap<CreateCouponRequestDto, Coupon>()
                .ForMember(d => d.UsedCount, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore());

            // Update maps only non-null fields (partial update).
            CreateMap<UpdateCouponRequestDto, Coupon>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
