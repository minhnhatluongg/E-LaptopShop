using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}
