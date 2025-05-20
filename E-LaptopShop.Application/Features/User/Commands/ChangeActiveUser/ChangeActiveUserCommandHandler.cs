using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Commands.ChangeActiveUser
{
    public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ChangeUserStatusCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.ChangeActiveAsync(request.Id, request.IsActive, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}
