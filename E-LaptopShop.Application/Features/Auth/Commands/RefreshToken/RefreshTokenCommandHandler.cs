using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Domain.Repositories;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IUserAuthRepository _userAuthRepository;

        public RefreshTokenCommandHandler(
            IJwtService jwtService,
            IUserAuthRepository userAuthRepository)
        {
            _jwtService = jwtService;
            _userAuthRepository = userAuthRepository;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Use JWT service to refresh token
                var tokenResponse = await _jwtService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

                // Get user info from new access token
                var userId = _jwtService.GetUserIdFromToken(tokenResponse.AccessToken);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                var user = await _userAuthRepository.GetByIdWithRoleAsync(userIdInt, cancellationToken);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                return new AuthResponseDto
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    AccessTokenExpiration = tokenResponse.AccessTokenExpiration,
                    RefreshTokenExpiration = tokenResponse.RefreshTokenExpiration,
                    ExpiresIn = tokenResponse.ExpiresIn,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role?.Name ?? "Customer",
                        EmailConfirmed = user.EmailConfirmed,
                        AvatarUrl = user.AvatarUrl
                    }
                };
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Failed to refresh token: " + ex.Message);
            }
        }
    }
}
