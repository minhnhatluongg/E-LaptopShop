using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage
{
    public record UpdateProductImageCommand : IRequest<ProductImageDto>
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ImageUrl { get; init; } = null!;
        public string FileType { get; init; } = null!;
        public long FileSize { get; init; }
        public bool IsMain { get; init; }
        public int DisplayOrder { get; init; }
        public string? AltText { get; init; }
        public string? Title { get; init; }
    }
}
