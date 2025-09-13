using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetById
{
    public class GetUserAddressByIdQueryHandler : IRequestHandler<GetUserAddressByIdQuery, UserAddressDto>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMapper _mapper;
        public GetUserAddressByIdQueryHandler(IUserAddressRepository userAddressRepository, IMapper mapper)
        {
            _userAddressRepository = userAddressRepository;
            _mapper = mapper;
        }
        public async Task<UserAddressDto> Handle(GetUserAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var e = await _userAddressRepository.FindAsync(request.Id, request.UserId, cancellationToken);
            if (e == null) Throw.NotFound(nameof(Domain.Entities.UserAddress), request.Id);
            return _mapper.Map<UserAddressDto>(e);
        }
    }
}
