using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Command.DeleteRole
{
    public class DeleteRoleCommand : IRequest<int>
    {
        public int Id { get; set; }
    }
}
