using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<Domain.Entities.User>(request);

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);

            return _mapper.Map<UserDto>(createdUser);
        }
    }
}
