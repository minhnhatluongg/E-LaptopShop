using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRoleRepository _roleRepository;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IUserAuthRepository userAuthRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _userAuthRepository = userAuthRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _roleRepository = roleRepository;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if email already exists
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email is already registered");
                }
            }
            catch (KeyNotFoundException)
            {
                // Email doesn't exist, continue with registration
            }

            // 2. Get default role (assuming "Customer" role exists)
            var userRole = await _roleRepository.GetByNameAsync("Customer", cancellationToken);
            if (userRole == null)
            {
                throw new InvalidOperationException("Default customer role not found");
            }

            // 3. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 4. Create user entity
            var newUser = new Domain.Entities.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.Phone,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                RoleId = userRole.Id,
                IsActive = true,
                EmailConfirmed = false, // 2025: Require email confirmation
                CreatedAt = DateTime.UtcNow
            };

            // 5. Save user to database
            var createdUser = await _userRepository.AddAsync(newUser, cancellationToken);

            // 6. Get user with role for token generation
            var userWithRole = await _userAuthRepository.GetByIdWithRoleAsync(createdUser.Id, cancellationToken);
            if (userWithRole == null)
            {
                throw new InvalidOperationException("Failed to retrieve created user");
            }

            // 7. Generate tokens
            var tokenResponse = await _jwtService.GenerateTokensAsync(userWithRole, cancellationToken);

            // 8. Return auth response
            return new AuthResponseDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                AccessTokenExpiration = tokenResponse.AccessTokenExpiration,
                RefreshTokenExpiration = tokenResponse.RefreshTokenExpiration,
                ExpiresIn = tokenResponse.ExpiresIn,
                User = new UserInfoDto
                {
                    Id = userWithRole.Id,
                    Email = userWithRole.Email,
                    FullName = userWithRole.FullName,
                    Role = userWithRole.Role?.Name ?? "Customer",
                    EmailConfirmed = userWithRole.EmailConfirmed,
                    AvatarUrl = userWithRole.AvatarUrl
                }
            };
        }
    }
}
