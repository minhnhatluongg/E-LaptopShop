using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Domain.Repositories.IPasswordHasher _passwordHasher;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            Domain.Repositories.IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            var user = _mapper.Map<Domain.Entities.User>(request);

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            }
            else
            {
                user.PasswordHash = existingUser.PasswordHash;
            }

            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

            return _mapper.Map<UserDto>(updatedUser);
        }
    }
}
