using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for ProductImage entity and related DTOs
    /// Handles mapping between entity, request DTOs, and response DTOs
    /// </summary>
    public class ProductImageMappingProfile : Profile
    {
        public ProductImageMappingProfile() 
        {
            // Entity to DTO mappings
            CreateMap<ProductImage, ProductImageDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            // Create DTO to Entity mappings
            CreateMap<CreateProductImageRequestDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.SysFile, opt => opt.Ignore());

            // Update DTO to Entity mappings
            CreateMap<UpdateProductImageRequestDto, ProductImage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.SysFile, opt => opt.Ignore());

            // Command to DTO mappings for backward compatibility
            CreateMap<Features.ProductImage.Commands.CreateProductImage.CreateProductImageCommand, CreateProductImageRequestDto>();
            CreateMap<Features.ProductImage.Commands.UpdateProductImage.UpdateProductImageCommand, UpdateProductImageRequestDto>();

            // Legacy DTOs (for backward compatibility)
            CreateMap<CreateProductImageRequestDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.SysFile, opt => opt.Ignore());

            CreateMap<UpdateProductImageRequestDto, ProductImage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.SysFile, opt => opt.Ignore());
        }
    }
}
