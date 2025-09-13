using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.CreateUserAddress
{
    public class CreateUserAddressCommandHandler : IRequestHandler<CreateUserAddressCommand, UserAddressDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserAddressRepository _userAddressRepository;

        public CreateUserAddressCommandHandler(IMapper mapper, IUserAddressRepository userAddressRepository)
        {
            _mapper = mapper;
            _userAddressRepository = userAddressRepository;
        }
        public async Task<UserAddressDto> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
        {
            if(request.IsDefault)
            {
                await _userAddressRepository.ClearDefaultAsync(request.UserId, cancellationToken);
            }
            var entity = _mapper.Map<Domain.Entities.UserAddress>(request);
            await _userAddressRepository.AddAsync(entity, cancellationToken);
            await _userAddressRepository.SaveChangesAsync(cancellationToken);
            var dto = _mapper.Map<UserAddressDto>(entity);
            return dto;
        }
    }
}
