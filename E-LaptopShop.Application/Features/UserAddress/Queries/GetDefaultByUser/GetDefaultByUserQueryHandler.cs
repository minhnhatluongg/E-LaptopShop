using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetDefaultByUser
{
    public class GetDefaultByUserQueryHandler : IRequestHandler<GetDefaultByUserQuery, UserAddressDto?>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMapper _mapper;
        public GetDefaultByUserQueryHandler(IUserAddressRepository userAddressRepository, IMapper mapper)
        {   
            _userAddressRepository = userAddressRepository;
            _mapper = mapper;
        }
        public async Task<UserAddressDto?> Handle(GetDefaultByUserQuery request, CancellationToken cancellationToken)
        {
            var q = await _userAddressRepository.Query()
                .Where(x => x.UserId == request.UserId && x.IsDefault == true)
                .FirstOrDefaultAsync(cancellationToken);
            if (q == null) Throw.NotFound(nameof(Domain.Entities.UserAddress), request.UserId);
            return _mapper.Map<UserAddressDto>(q);
        }
    }
}
