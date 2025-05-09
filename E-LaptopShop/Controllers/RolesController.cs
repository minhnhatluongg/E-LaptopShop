using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Roles.Command.CreateRole;
using E_LaptopShop.Application.Features.Roles.Command.DeleteRole;
using E_LaptopShop.Application.Features.Roles.Command.UpdateRole;
using E_LaptopShop.Application.Features.Roles.Queries.GetAllRoles;
using E_LaptopShop.Application.Features.Roles.Queries.GetRoleById;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public string EntityName => "Role";
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetAllRoles([FromQuery] GetAllRolesQuery query)
        {
            var roles = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(roles));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetById(int id)
        {
            var role = await _mediator.Send(new GetRoleById { Id = id });
            return Ok(ApiResponse<RoleDto>.SuccessResponse(role));
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> Create([FromBody] CreateRoleCommand command)
        {
            var role = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = role.Id }, 
                ApiResponse<RoleDto>.SuccessResponse(role,$"{EntityName} created successfully"));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> Update(int id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            var role = await _mediator.Send(command);
            return Ok(ApiResponse<RoleDto>.SuccessResponse(role, $"{EntityName} updated successfully"));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> Delete (int id)
        {
            var role = await _mediator.Send(new DeleteRoleCommand { Id = id });
            return Ok(ApiResponse<int>.SuccessResponse(role, $"{EntityName} deleted successfully"));
        }
    }
}
