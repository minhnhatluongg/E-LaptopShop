using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommand : IRequest<bool>
    {
        public int Id { get; init; }
    }
}
