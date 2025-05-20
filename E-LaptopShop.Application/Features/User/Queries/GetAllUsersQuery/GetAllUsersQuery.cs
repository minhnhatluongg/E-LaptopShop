using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.GetAllUsersQuery
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? Gender { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
        public string? SearchTerm { get; set; }
    }
}
