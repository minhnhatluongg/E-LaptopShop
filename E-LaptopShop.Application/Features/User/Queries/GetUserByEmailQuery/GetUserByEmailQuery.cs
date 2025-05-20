using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.GetUserByEmailQuery
{
    public class GetUserByEmailQuery : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;
    }
}
