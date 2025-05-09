using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleById : IRequest<RoleDto>
    {
        public int Id { get; set; }
    }
}
