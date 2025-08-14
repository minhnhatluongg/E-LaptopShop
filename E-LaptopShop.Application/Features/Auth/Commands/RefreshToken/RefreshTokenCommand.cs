using E_LaptopShop.Application.DTOs.Auth;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
