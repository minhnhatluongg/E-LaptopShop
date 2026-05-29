using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            CreateMap<UserWallet, WalletDto>();
            CreateMap<WalletTransaction, WalletTransactionDto>();
        }
    }
}
