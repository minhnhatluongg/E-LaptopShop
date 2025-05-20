using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.User.Commands.ChangeActiveUser;
using E_LaptopShop.Application.Features.User.Commands.CreateUser;
using E_LaptopShop.Application.Features.User.Commands.DeleteUser;
using E_LaptopShop.Application.Features.User.Commands.UpdateUser;
using E_LaptopShop.Application.Features.User.Queries.CheckEmailExistsQuery;
using E_LaptopShop.Application.Features.User.Queries.GetAllUsersQuery;
using E_LaptopShop.Application.Features.User.Queries.GetPagedUsersQuery;
using E_LaptopShop.Application.Features.User.Queries.GetUserByEmailQuery;
using E_LaptopShop.Application.Features.User.Queries.GetUserByIdQuery;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;
        public string EntityName => "User";

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng với các bộ lọc tùy chọn
        /// </summary>
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers([FromQuery] GetAllUsersQuery query)
        {
            try
            {
                var users = await _mediator.Send(query);
                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting user {id}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo email
        /// </summary>
        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByEmailQuery { Email = email });
                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting user by email {email}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng có phân trang
        /// </summary>
        [HttpGet("GetPagedUsers")]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetPagedUsers([FromQuery] GetPagedUsersQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged users");
                return StatusCode(500, ApiResponse<PagedResult<UserDto>>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Kiểm tra xem email đã tồn tại chưa
        /// </summary>
        [HttpGet("CheckEmailExists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists([FromQuery] string email, [FromQuery] int? excludeId = null)
        {
            try
            {
                var query = new CheckEmailExistsQuery { Email = email, ExcludeId = excludeId };
                var exists = await _mediator.Send(query);
                return Ok(ApiResponse<bool>.SuccessResponse(exists));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if email exists: {email}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        [HttpPost("CreateUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserCommand command)
        {
            try
            {
                var user = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = user.Id },
                    ApiResponse<UserDto>.SuccessResponse(user, $"{EntityName} created successfully"));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email"))
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, [FromBody] UpdateUserCommand command)
        {
            try
            {
                if (id != command.Id)
                    return BadRequest(ApiResponse<UserDto>.ErrorResponse("ID mismatch"));

                var user = await _mediator.Send(command);
                return Ok(ApiResponse<UserDto>.SuccessResponse(user, $"{EntityName} updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email"))
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating user {id}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteUserCommand { Id = id });
                return Ok(ApiResponse<int>.SuccessResponse(result, $"{EntityName} deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<int>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting user {id}");
                return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Thay đổi trạng thái kích hoạt của người dùng
        /// </summary>
        [HttpPut("ChangeUserStatus/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> ChangeStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var command = new ChangeUserStatusCommand { Id = id, IsActive = isActive };
                var user = await _mediator.Send(command);
                return Ok(ApiResponse<UserDto>.SuccessResponse(user, $"{EntityName} status changed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while changing status for user {id}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }
    }
}
