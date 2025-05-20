using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Commands.ChangeActiveUser
{
    public class ChangeUserStatusCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
