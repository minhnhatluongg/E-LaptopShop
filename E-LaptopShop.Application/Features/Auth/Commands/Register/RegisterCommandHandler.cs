using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private IAuthService _authService;
        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = new RegisterRequestDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth
            };
            await _authService.RegisterAsync(dto, cancellationToken);
            return await _authService.LoginAsync(new LoginRequestDto
            {
                Email = request.Email,
                Password = request.Password
            }, cancellationToken);
        }
    }
}
