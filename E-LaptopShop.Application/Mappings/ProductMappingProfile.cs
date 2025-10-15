using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Features.Products.Commands.CreateProduct;
using E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;
using E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Application.Mappings;

public class ProductMappingProfile : Profile
{

    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<CreateProductRequestDto, Product>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            //.ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Name, o => o.MapFrom(s => (s.Name ?? string.Empty).Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => (s.Description ?? string.Empty).Trim()));
        // Hoặc: .ForMember(d => d.Description, o => o.NullSubstitute(string.Empty));
        CreateMap<GetAllProductsQuery, ProductQueryParams>();

        CreateMap<UpdateProductRequestDto, Product>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            //.ForMember(d => d.UpdatedAt, o => o.MapFrom((src, dst, _, ctx) =>
            //ctx.Items.TryGetValue("Now", out var now) && now is DateTime dt ? dt : DateTime.UtcNow))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => (s.Description ?? string.Empty).Trim()));
        // Nếu cần partial update, bật:
        // .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
    }
}
