using E_LaptopShop.Application.DTOs.Auth;
using E_LaptopShop.Application.Features.Auth.Commands.Login;
using E_LaptopShop.Application.Features.Auth.Commands.Logout;
using E_LaptopShop.Application.Features.Auth.Commands.RefreshToken;
using E_LaptopShop.Application.Features.Auth.Commands.Register;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_LaptopShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 🔐 User Login - Đăng nhập người dùng
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>JWT tokens và thông tin user</returns>
        [HttpPost("login")]
        [Tags("🔐 Authentication")]
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
        [Tags("🔐 Authentication")]
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

                // 2025 Security: Set refresh token as HTTP-only cookie
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
        [Tags("🔐 Authentication")]
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
        [Tags("👤 Customer")]
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
        /// 👤 Get Current User Info - Lấy thông tin user hiện tại
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("me")]
        [Authorize]
        [Tags("👤 Customer")]
        [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
        {
            try
            {
                var userInfo = new UserInfoDto
                {
                    Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                    Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                    FullName = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "",
                    EmailConfirmed = bool.Parse(User.FindFirst("email_confirmed")?.Value ?? "false")
                };

                return Ok(ApiResponse<UserInfoDto>.SuccessResponse(userInfo, "User info retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user info");
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve user info"));
            }
        }

        /// <summary>
        /// 🔒 Revoke All Tokens - Thu hồi tất cả refresh tokens của user
        /// </summary>
        /// <returns>Revoke status</returns>
        [HttpPost("revoke-all-tokens")]
        [Authorize]
        [Tags("👤 Customer")]
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
