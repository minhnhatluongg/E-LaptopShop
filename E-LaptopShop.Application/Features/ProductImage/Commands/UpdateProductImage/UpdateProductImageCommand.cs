using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage
{
    /// <summary>
    /// Command for updating an existing ProductImage
    /// Contains all necessary data for ProductImage update
    /// </summary>
    public record UpdateProductImageCommand : IRequest<ProductImageDto>
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public int? SysFileId { get; init; }
        public string ImageUrl { get; init; } = null!;
        public string FileType { get; init; } = null!;
        public long FileSize { get; init; }
        public bool IsMain { get; init; }
        public int DisplayOrder { get; init; }
        public string? AltText { get; init; }
        public string? Title { get; init; }
        public bool IsActive { get; init; } = true;
    }
}
