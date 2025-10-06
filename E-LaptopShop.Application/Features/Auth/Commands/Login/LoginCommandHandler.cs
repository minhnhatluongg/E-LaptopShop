using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace E_LaptopShop.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;
        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return _authService.LoginAsync(new LoginRequestDto
            {
                Email = request.Email?.Trim(),
                Password = request.Password
            }, cancellationToken);
        }
    }
}
