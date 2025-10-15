using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommand : IRequest<BrandDto>
    {
        public CreateBrandRequestDto RequestDto { get; init; } = null!;
    }
}
