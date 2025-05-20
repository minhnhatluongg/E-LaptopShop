using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.CheckEmailExistsQuery
{
    public class CheckEmailExistsQuery : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public int? ExcludeId { get; set; }
    }
}
