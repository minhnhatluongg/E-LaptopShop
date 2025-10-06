using E_LaptopShop.Application.Services;
using MediatR;

namespace E_LaptopShop.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IJwtService _jwtService;

        public LogoutCommandHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _jwtService.RevokeTokenAsync(request.UserId, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
