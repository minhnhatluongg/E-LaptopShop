using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.SetMainImage
{
    public record SetMainImageCommand : IRequest<ProductImageDto>
    {
        public int Id { get; init; }
        public SetMainImageCommand(int id)
        {
            Id = id;
        }
    }
}
