using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
        public int? Id { get; init; }
        public string? Name { get; init; }
        public bool? IsActive { get; init; }
    }
}
