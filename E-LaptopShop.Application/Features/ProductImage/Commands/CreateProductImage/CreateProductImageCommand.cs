using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage
{
    /// <summary>
    /// Command for creating a new ProductImage
    /// Contains all necessary data for ProductImage creation
    /// </summary>
    public record CreateProductImageCommand : IRequest<ProductImageDto>
    {
        public int ProductId { get; init; }
        public int? SysFileId { get; init; }
        public string ImageUrl { get; init; } = null!;
        public string FileType { get; init; } = null!;
        public long FileSize { get; init; }
        public bool IsMain { get; init; }
        public int DisplayOrder { get; init; } = 0;
        public string? AltText { get; init; }
        public string? Title { get; init; }
        public string? CreatedBy { get; init; }
    }
}