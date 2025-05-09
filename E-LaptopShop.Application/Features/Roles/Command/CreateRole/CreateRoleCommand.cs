using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Command.CreateRole
{
    public record CreateRoleCommand : IRequest<RoleDto>
    {
        public string Name { get; init; } = null!;
        public bool IsActive { get; init; } = true;
    }
}
