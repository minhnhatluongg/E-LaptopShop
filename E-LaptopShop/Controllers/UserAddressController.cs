using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.UserAddress.Commands.CreateUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Commands.DeleteUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Commands.HardDeleteUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Commands.SetDefaultUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Commands.UpdateUserAddress;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetAllFilteredPaged;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetById;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetByUser;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetDefaultByUser;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetDeletedHavePaged;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetExist;
using E_LaptopShop.Application.Features.UserAddress.Queries.GetStatics;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly ILogger<UserAddressController> _logger;
        private readonly IMediator _mediator;
        public UserAddressController(
            IMediator mediator,
            ILogger<UserAddressController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo địa chỉ (address) cho người dùng
        /// </summary>
        /// api/user-address
        [ProducesResponseType(typeof(ApiResponse<UserAddressDto>), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserAddressDto>>> CreateUserAddress([FromBody] CreateUserAddressCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(ApiResponse<UserAddressDto>.SuccessResponse(result, "Create UserAddress successfully!."));
        }
        /// <summary>
        /// 
        // PUT api/user-addresses/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserAddressDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserAddressCommand cmd, CancellationToken ct)
        {
            if (id != cmd.Id) return BadRequest("Route id và payload id không khớp.");
            var result = await _mediator.Send(cmd, ct);
            return Ok(ApiResponse<UserAddressDto>.SuccessResponse(result, "Create UserAddress successfully!."));
        }

        // DELETE (soft) api/user-addresses/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteUserAddressCommand { Id = id }, ct);
            return Ok(ApiResponse<string>.SuccessResponse("", "Delete UserAddress successfully!."));
        }

        // DELETE (hard) api/user-addresses/{id}/hard
        [HttpDelete("{id:int}/hard")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> HardDelete(int id, CancellationToken ct)
        {
            await _mediator.Send(new hardDeleteUserAddressCommand { Id = id }, ct);
            return Ok(ApiResponse<string>.SuccessResponse("", "Hard delete UserAddress successfully!."));
        }

        // PUT set default  api/user-addresses/{id}/default
        [HttpPut("{id:int}/default")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SetDefault(int id, [FromQuery] int userId, CancellationToken ct)
        {
            await _mediator.Send(new SetDefaultUserAddressCommand { Id = id, UserId = userId }, ct);
            return Ok(ApiResponse<string>.SuccessResponse("", "Set default UserAddress successfully!."));
        }

        //GET api/user-addresses/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<UserAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUserAddressByIdQuery { Id = id }, ct);
            return Ok(ApiResponse<UserAddressDto>.SuccessResponse(result, "Get UserAddress by id successfully!."));
        }

        // GET api/user-addresses/user/{userId}
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<UserAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(int userId, CancellationToken ct)
        {
            var list = await _mediator.Send(new GetUserAddressesQuery { UserId = userId }, ct);
            return Ok(ApiResponse<IReadOnlyList<UserAddressDto>>.SuccessResponse(list, "Get UserAddresses by userId successfully!."));
        }

        // GET api/user-addresses/user/{userId}/default
        [HttpGet("user/{userId:int}/default")]
        [ProducesResponseType(typeof(UserAddressDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDefaultByUser(int userId, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetDefaultByUserQuery { UserId = userId }, ct);
            return Ok(ApiResponse<UserAddressDto>.SuccessResponse(dto, "Get default UserAddress by userId successfully!."));
        }

        // GET (paged filter) api/user-addresses
        // ?pageNumber=1&pageSize=20&sortBy=CreatedAt&sortOrder=desc&search=abc&city=Hanoi&isDefault=true
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFilteredPaged([FromQuery] GetAllFilteredPagedQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(query, ct);
            return Ok(ApiResponse<PagedResult<UserAddressDto>>.SuccessResponse(result, "Get UserAddresses paged filter successfully!."));
        }

        // GET (deleted paged) api/user-addresses/deleted
        [HttpGet("deleted")]
        [ProducesResponseType(typeof(PagedResult<UserAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeletedPaged([FromQuery] GetDeletedUserAddressesPagedQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(query, ct);
            return Ok(ApiResponse<PagedResult<UserAddressDto>>.SuccessResponse(result, "Get deleted UserAddresses paged successfully!."));
        }

        // GET /api/user-addresses/exists?userId=1&phone=0901234567&addressLine=123 ABC
        [HttpGet("exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByUserPhone(
            [FromQuery] int userId,
            [FromQuery] string phone,
            [FromQuery] string? addressLine,
            CancellationToken ct)
        {
            var exists = await _mediator.Send(new ExistsByUserPhoneQuery
            {
                UserId = userId,
                Phone = phone,
                AddressLine = addressLine
            }, ct);

            return Ok(ApiResponse<bool>.SuccessResponse(exists, "Check exists UserAddress by userId and phone successfully!."));
        }

        // GET api/user-addresses/statistics
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(UserAddressStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Statistics(CancellationToken ct)
        {
            var stat = await _mediator.Send(new GetUserAddressStatsQuery(), ct);
            return Ok(ApiResponse<UserAddressStatsDto>.SuccessResponse(stat, "Get UserAddress statistics successfully!."));
        }
    }
}
