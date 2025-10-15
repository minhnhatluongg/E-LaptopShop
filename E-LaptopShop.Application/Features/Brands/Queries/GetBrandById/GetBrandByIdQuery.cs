using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Queries.GetBrandById
{
    public class GetBrandByIdQuery : IRequest<BrandDto?>
    {
        public int Id { get; init; }
    }
}
