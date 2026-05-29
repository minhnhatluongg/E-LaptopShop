using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Features.Auth.Commands.Login;
using E_LaptopShop.Application.Features.Auth.Commands.Logout;
using E_LaptopShop.Application.Features.Auth.Commands.RefreshToken;
using E_LaptopShop.Application.Features.Auth.Commands.Register;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using E_LaptopShop.Helpers;
using E_LaptopShop.Controllers.Api_version;

namespace E_LaptopShop.Controllers
{
    public class AuthController : ApiV1ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISysFileRepository _sysFileRepository;

        public AuthController(
            IMediator mediator,
            ILogger<AuthController> logger,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ISysFileRepository sysFileRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _sysFileRepository = sysFileRepository;
        }

        private int GetCurrentUserId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        /// <summary>
        /// 🔐 User Login - Đăng nhập người dùng
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>JWT tokens và thông tin user</returns>
        [HttpPost("login")]
        [Tags(ApiTags.Authentication)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var command = new LoginCommand
                {
                    Email = request.Email,
                    Password = request.Password,
                    RememberMe = request.RememberMe
                };

                var result = await _mediator.Send(command);

                // 2025 Security: Set refresh token as HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Expires = result.RefreshTokenExpiration
                };

                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

                _logger.LogInformation("User {Email} logged in successfully", request.Email);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials or account locked"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", request.Email);
                return BadRequest(ApiResponse<object>.ErrorResponse("Login failed"));
            }
        }

        /// <summary>
        /// 📝 User Registration - Đăng ký tài khoản mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>JWT tokens và thông tin user mới</returns>
        [HttpPost("register")]
        [Tags(ApiTags.Authentication)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var command = new RegisterCommand
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Phone = request.Phone,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth
                };

                var result = await _mediator.Send(command);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Expires = result.RefreshTokenExpiration
                };

                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

                _logger.LogInformation("New user registered: {Email}", request.Email);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Registration successful"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", request.Email);
                _logger.LogError(ex, "Have problems {Problems}", ex);
                return BadRequest(ApiResponse<object>.ErrorResponse("Registration failed"));
            }
        }

        /// <summary>
        /// 🔄 Refresh Token - Làm mới access token
        /// </summary>
        /// <param name="request">Refresh token (hoặc từ cookie)</param>
        /// <returns>New JWT tokens</returns>
        [HttpPost("refresh-token")]
        [Tags(ApiTags.Authentication)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto? request = null)
        {
            try
            {
                // 2025 Enhancement: Accept refresh token from both body and HTTP-only cookie
                string? refreshToken = request?.RefreshToken;
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = Request.Cookies["refreshToken"];
                }

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Refresh token is required"));
                }

                var command = new RefreshTokenCommand { RefreshToken = refreshToken };
                var result = await _mediator.Send(command);

                // Update refresh token cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.RefreshTokenExpiration
                };

                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

                _logger.LogInformation("Token refreshed for user {UserId}", result.User.Id);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or expired refresh token"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return BadRequest(ApiResponse<object>.ErrorResponse("Token refresh failed"));
            }
        }

        /// <summary>
        /// 🚪 Logout - Đăng xuất và thu hồi tokens
        /// </summary>
        /// <returns>Logout status</returns>
        [HttpPost("logout")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid user"));
                }

                var command = new LogoutCommand { UserId = userId };
                var success = await _mediator.Send(command);

                // Remove refresh token cookie
                Response.Cookies.Delete("refreshToken");

                _logger.LogInformation("User {UserId} logged out", userId);

                return Ok(ApiResponse<object>.SuccessResponse(new { Success = success }, "Logout successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return BadRequest(ApiResponse<object>.ErrorResponse("Logout failed"));
            }
        }

        /// <summary>
        /// 👤 Get Current User Info - Lấy thông tin user hiện tại (từ JWT claims)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), 200)]
        public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
        {
            try
            {
                var userInfo = new UserInfoDto
                {
                    Id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                    Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
                    FullName = User.FindFirstValue(ClaimTypes.Name) ?? "",
                    Role = User.FindFirstValue(ClaimTypes.Role) ?? "",
                    EmailConfirmed = bool.Parse(User.FindFirstValue("email_confirmed") ?? "false")
                };

                // Lấy thêm AvatarUrl từ DB (không có trong JWT)
                var user = await _userRepository.GetByIdAsync(userInfo.Id);
                if (user != null)
                {
                    userInfo.AvatarUrl = user.AvatarUrl;
                }

                return Ok(ApiResponse<UserInfoDto>.SuccessResponse(userInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user info");
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve user info"));
            }
        }

        /// <summary>
        /// 👤 [CUSTOMER] Lấy full profile của bản thân
        /// </summary>
        [HttpGet("me/profile")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

                var dto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    AvatarUrl = user.AvatarUrl,
                    RoleId = user.RoleId,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                };
                return Ok(ApiResponse<UserDto>.SuccessResponse(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile for user");
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Failed to get profile"));
            }
        }

        /// <summary>
        /// 👤 [CUSTOMER] Cập nhật thông tin cá nhân (firstName, lastName, phone, gender, dateOfBirth)
        /// </summary>
        [HttpPut("me/profile")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateMyProfile([FromBody] UpdateMyProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

                if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName.Trim();
                if (!string.IsNullOrWhiteSpace(dto.LastName))  user.LastName  = dto.LastName.Trim();
                if (dto.Phone != null)       user.Phone = dto.Phone.Trim();
                if (dto.Gender != null)      user.Gender = dto.Gender;
                if (dto.DateOfBirth != null) user.DateOfBirth = dto.DateOfBirth;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId.ToString();

                await _userRepository.UpdateAsync(user);

                var result = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    AvatarUrl = user.AvatarUrl,
                    RoleId = user.RoleId,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                };
                return Ok(ApiResponse<UserDto>.SuccessResponse(result, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Failed to update profile"));
            }
        }

        /// <summary>
        /// 🔒 [CUSTOMER] Đổi mật khẩu — xác minh mật khẩu hiện tại trước.
        /// </summary>
        [HttpPut("me/password")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Mật khẩu mới và xác nhận không khớp"));

                if (dto.NewPassword.Length < 6)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Mật khẩu mới phải tối thiểu 6 ký tự"));

                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, dto.CurrentPassword))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Mật khẩu hiện tại không đúng"));

                user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId.ToString();
                await _userRepository.UpdateAsync(user);

                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Đổi mật khẩu thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to change password"));
            }
        }

        /// <summary>
        /// 🖼️ [CUSTOMER] Cập nhật avatar — upload file trước qua /api/v1/file/upload-file,
        /// nhận về SysFileId, rồi gọi endpoint này.
        /// </summary>
        [HttpPut("me/avatar")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAvatar([FromBody] UpdateAvatarDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var sysFile = await _sysFileRepository.GetByIdAsync(dto.SysFileId);
                if (sysFile == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("File không tồn tại, upload lại."));

                user.AvatarUrl = sysFile.FileUrl;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId.ToString();
                await _userRepository.UpdateAsync(user);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { avatarUrl = sysFile.FileUrl },
                    "Cập nhật avatar thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating avatar");
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update avatar"));
            }
        }

        /// <summary>
        /// 🗑️ [CUSTOMER] Xoá ảnh đại diện — set AvatarUrl về null.
        /// </summary>
        [HttpDelete("me/avatar")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAvatar()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                user.AvatarUrl = null;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId.ToString();
                await _userRepository.UpdateAsync(user);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { avatarUrl = (string?)null },
                    "Đã xoá ảnh đại diện"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar");
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete avatar"));
            }
        }

        /// <summary>
        /// 🔒 Revoke All Tokens - Thu hồi tất cả refresh tokens của user
        /// </summary>
        /// <returns>Revoke status</returns>
        [HttpPost("revoke-all-tokens")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<ActionResult<ApiResponse<object>>> RevokeAllTokens()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid user"));
                }

                var command = new LogoutCommand { UserId = userId };
                await _mediator.Send(command);

                // Remove refresh token cookie
                Response.Cookies.Delete("refreshToken");

                _logger.LogInformation("All tokens revoked for user {UserId}", userId);

                return Ok(ApiResponse<object>.SuccessResponse(new { Success = true }, "All tokens revoked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens");
                return BadRequest(ApiResponse<object>.ErrorResponse("Token revocation failed"));
            }
        }
    }
}
