using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetStatics
{
    public class GetUserAddressStatsQueryHandler : IRequestHandler<GetUserAddressStatsQuery, UserAddressStatsDto>
    {
        private readonly IUserAddressRepository _repoUserAdress;
        public GetUserAddressStatsQueryHandler(IUserAddressRepository userAddressRepository)
        {
            _repoUserAdress = userAddressRepository;
        }
        public async Task<UserAddressStatsDto> Handle(GetUserAddressStatsQuery request, CancellationToken cancellationToken)
        {
            var q = _repoUserAdress.QueryIgnoreFilters();

            if(request.UserId.HasValue)
            {
                q = q.Where(x => x.UserId == request.UserId.Value);
            }
            var total = await q.CountAsync(cancellationToken);
            var active = await q.Where(x => !x.IsDeleted).CountAsync(cancellationToken);
            var deleted = await q.Where(x => x.IsDeleted).CountAsync(cancellationToken);
            var defaultCount = await 
                q.Where(x => x.IsDefault == true && !x.IsDeleted).CountAsync(cancellationToken);

            return new UserAddressStatsDto
            {
                Total = total,
                Active = active,
                Deleted = deleted,
                DefaultCount = defaultCount
            };
        }
    }
}
