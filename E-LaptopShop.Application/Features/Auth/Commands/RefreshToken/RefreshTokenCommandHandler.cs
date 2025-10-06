using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;
        public RefreshTokenCommandHandler(IAuthService authService) => _authService = authService;
        public Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return _authService.RefreshTokenAsync(new RefreshTokenRequestDto { RefreshToken = request.RefreshToken }, cancellationToken);
        }
    }
}
