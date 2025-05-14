using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetImagesByProductId
{
    public record GetImagesByProductIdQuery : IRequest<IEnumerable<ProductImageDto>>
    {
        public int ProductId { get; init; }
    }
}
