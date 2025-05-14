using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage
{
    public record CreateProductImageCommand : IRequest<ProductImageDto>
    {
        public int ProductId { get; init; }
        public string ImageUrl { get; init; } = null!;
        public string FileType { get; init; } = null!;
        public long FileSize { get; init; }
        public bool IsMain { get; init; }
        public int DisplayOrder { get; init; }
        public string? AltText { get; init; }
        public string? Title { get; init; }
        public string? CreatedBy { get; init; }
    }
}