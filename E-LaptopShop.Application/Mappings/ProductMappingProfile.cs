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
        CreateMap<Product, ProductDto>()
            // Brand
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand != null ? s.Brand.Name : null))
            .ForMember(d => d.BrandSlug, o => o.MapFrom(s => s.Brand != null ? s.Brand.Slug : null))
            // Category
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            // Ảnh chính — 1 correlated subquery EF Core translate được:
            // ORDER BY IsMain DESC → ảnh IsMain=true lên đầu, fallback về ảnh DisplayOrder nhỏ nhất.
            .ForMember(d => d.MainImageUrl, o => o.MapFrom(s =>
                s.ProductImages
                    .Where(i => i.IsActive)
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()))
            // Specs tóm tắt — lấy bản ghi đầu tiên của ProductSpecifications
            .ForMember(d => d.Cpu,     o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.CPU).FirstOrDefault()))
            .ForMember(d => d.Gpu,     o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.GPU).FirstOrDefault()))
            .ForMember(d => d.Ram,     o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.RAM).FirstOrDefault()))
            .ForMember(d => d.Storage, o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.Storage).FirstOrDefault()))
            .ForMember(d => d.Screen,  o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.Screen).FirstOrDefault()))
            .ForMember(d => d.Battery, o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.Battery).FirstOrDefault()))
            .ForMember(d => d.Weight,  o => o.MapFrom(s => s.ProductSpecifications.Select(x => x.Weight).FirstOrDefault()))
            // Review summary — đếm và trung bình từ ProductReviews
            .ForMember(d => d.TotalReviews, o => o.MapFrom(s => s.ProductReviews.Count(r => r.Rating != null)))
            .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                s.ProductReviews.Where(r => r.Rating != null).Any()
                    ? s.ProductReviews.Where(r => r.Rating != null).Average(r => (double)r.Rating!.Value)
                    : 0d))
            // Comments count — đếm ProductComments chưa bị xóa
            .ForMember(d => d.TotalComments, o => o.MapFrom(s => s.ProductComments.Count(c => !c.IsDeleted)));

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
