using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
        Task<UserDto> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    }
}
