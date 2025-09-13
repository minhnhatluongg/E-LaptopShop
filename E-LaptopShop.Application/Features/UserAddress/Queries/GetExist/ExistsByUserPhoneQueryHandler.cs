using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetExist
{
    public class ExistsByUserPhoneQueryHandler : IRequestHandler<ExistsByUserPhoneQuery, bool>
    {
        private readonly IUserAddressRepository _repoUserAddress;
        public ExistsByUserPhoneQueryHandler(IUserAddressRepository repo)
        {
            _repoUserAddress = repo;
        }
        public Task<bool> Handle(ExistsByUserPhoneQuery request, CancellationToken ct)
        {
            return _repoUserAddress.Query().AnyAsync(x =>
                        x.UserId == request.UserId &&
                        x.Phone == request.Phone &&
                        (request.AddressLine == null || x.AddressLine == request.AddressLine), ct);
        }
    }
}
