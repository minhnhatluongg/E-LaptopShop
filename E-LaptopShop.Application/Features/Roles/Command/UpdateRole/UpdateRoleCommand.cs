using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Command.UpdateRole
{
    public class UpdateRoleCommand : IRequest<RoleDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
