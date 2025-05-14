using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.DeleteProductImgages
{
    public record DeleteProductImageCommand : IRequest<int>
    {
        public int Id { get; set; }
        public DeleteProductImageCommand(int id )
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            }
            Id = id;
        }
    }
}
