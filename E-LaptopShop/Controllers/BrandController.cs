using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Features.Brands.Commands.CreateBrand;
using E_LaptopShop.Application.Features.Brands.Commands.DeleteBrand;
using E_LaptopShop.Application.Features.Brands.Commands.UpdateBrand;
using E_LaptopShop.Application.Features.Brands.Queries.GetActiveBrandsQuery;
using E_LaptopShop.Application.Features.Brands.Queries.GetAllBrandsQuery;
using E_LaptopShop.Application.Features.Brands.Queries.GetBrandById;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Api.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public const string EntityName = "Brand";

        public BrandsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Tags("🔓 Public")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<BrandDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<PagedResult<BrandDto>>>> GetAll([FromQuery] BrandQueryParams queryParams)
        {
            var query = new GetAllBrandsQuery { QueryParams = queryParams };
            var brands = await _mediator.Send(query);

            return Ok(ApiResponse<PagedResult<BrandDto>>.SuccessResponse(brands));
        }


        [HttpGet("active")]
        [Tags("🔓 Public")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BrandDto>>), 200)]
        public async Task<IActionResult> GetActiveBrands()
        {
            var query = new GetActiveBrandsQuery();
            var brands = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<BrandDto>>.SuccessResponse(brands));
        }

        [HttpGet("{id}")]
        [Tags("🔓 Public")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            var brand = await _mediator.Send(new GetBrandByIdQuery { Id = id });
            return Ok(ApiResponse<BrandDto>.SuccessResponse(brand));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [Tags("👑 Admin")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateBrandRequestDto requestDto)
        {
            var command = new CreateBrandCommand { RequestDto = requestDto };
            var brand = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = brand.Id },
                ApiResponse<BrandDto>.SuccessResponse(brand, $"{EntityName} created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [Tags("👑 Admin")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateBrandRequestDto requestDto)
        {
            if (id != requestDto.Id)
            {
                return BadRequest(ApiResponse<ProductDto>.ErrorResponse("ID mismatch between route and body"));
            }

            var command = new UpdateBrandCommand { Id = id, RequestDto = requestDto };
            var brand = await _mediator.Send(command);

            return Ok(ApiResponse<BrandDto>.SuccessResponse(brand, $"{EntityName} updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Tags("👑 Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
        {
            var command = new DeleteBrandCommand { Id = id };
            var result = await _mediator.Send(command);

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = $"{EntityName} deleted successfully",
                Data = id
            });
        }
    }
}