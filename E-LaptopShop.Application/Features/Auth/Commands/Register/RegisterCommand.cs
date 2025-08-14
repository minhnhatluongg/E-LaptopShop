using E_LaptopShop.Application.DTOs.Auth;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
