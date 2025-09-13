using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetByUser
{
    public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, IReadOnlyList<UserAddressDto>>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMapper _mapper;

        public GetUserAddressesQueryHandler(IUserAddressRepository userAddressRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userAddressRepository = userAddressRepository;
        }
        public async Task<IReadOnlyList<UserAddressDto>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
        {
            var list = await _userAddressRepository.Query()
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.UpdatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<UserAddressDto>>(list);
        }
    }
}
