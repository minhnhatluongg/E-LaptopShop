using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace E_LaptopShop.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public LoginCommandHandler(
            IUserAuthRepository userAuthRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userAuthRepository = userAuthRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy user theo email
            var user = await _userAuthRepository.GetByEmailForAuthAsync(request.Email, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            // 2. Kiểm tra trạng thái khóa tài khoản
            if (await _userAuthRepository.IsUserLockedAsync(user.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts. Please try again later.");
            }
            // 3. Kiểm tra trạng thái kích hoạt
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is deactivated");
            }
            // 4. Kiểm tra mật khẩu
            var passwordValid = !string.IsNullOrEmpty(user.PasswordHash) &&
                                _passwordHasher.VerifyHashedPassword(request.Password,user.PasswordHash);

            if (!passwordValid)
            {
                // Tăng số lần đăng nhập sai
                await _userAuthRepository.IncrementLoginAttemptsAsync(user.Id, cancellationToken);

                // Khóa nếu vượt giới hạn
                var failedAttempts = user.LoginAttempts + 1;
                if (failedAttempts >= _jwtSettings.MaxFailedAccessAttempts)
                {
                    var lockUntil = DateTime.UtcNow.AddMinutes(_jwtSettings.LockoutDurationMinutes);
                    await _userAuthRepository.LockUserAsync(user.Id, lockUntil, cancellationToken);
                }

                throw new UnauthorizedAccessException("Invalid email or password");
            }
            // 5. Reset số lần đăng nhập sai khi thành công
            await _userAuthRepository.ResetLoginAttemptsAsync(user.Id, cancellationToken);

            // 6. Cập nhật thời gian đăng nhập cuối
            await _userAuthRepository.UpdateLastLoginAsync(user.Id, cancellationToken);

            // 7. Tạo token
            var tokenResponse = await _jwtService.GenerateTokensAsync(user, cancellationToken);

            // 8. Trả về kết quả
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

    }
}
