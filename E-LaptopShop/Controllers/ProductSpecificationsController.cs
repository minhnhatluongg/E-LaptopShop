using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.CreateProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.DeleteProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.UpdateProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetAllProductSpecifications;
using E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetProductSpecificationById;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_LaptopShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductSpecificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public string EntityName => "ProductSpecification";

    public ProductSpecificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy tất cả spec sản phẩm - Dành cho catalog
    /// </summary>
    [HttpGet("GetAllSpecifications")]
    [Tags("🔓 Public")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductSpecificationDto>>>> GetAll([FromQuery] GetAllProductSpecificationsQuery query)
    {
        var specs = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<ProductSpecificationDto>>.SuccessResponse(specs));
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy chi tiết spec sản phẩm - Dành cho catalog
    /// </summary>
    [HttpGet("GetSpecificationById/{id}")]
    [Tags("🔓 Public")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> GetById(int id)
    {
        var spec = await _mediator.Send(new GetProductSpecificationByIdQuery { Id = id });
        return Ok(ApiResponse<ProductSpecificationDto>.SuccessResponse(spec));
    }

    /// <summary>
    /// 👑 [ADMIN] Tạo spec sản phẩm mới
    /// </summary>
    [HttpPost("CreateSpecification")]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> Create([FromBody] CreateProductSpecificationCommand command)
    {
        var spec = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = spec.Id },
            ApiResponse<ProductSpecificationDto>.SuccessResponse(spec, $"{EntityName} created successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Cập nhật spec sản phẩm
    /// </summary>
    [HttpPut("UpdateSpecification/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> Update(int id, [FromBody] UpdateProductSpecificationCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<ProductSpecificationDto>.ErrorResponse("ID mismatch"));

        var spec = await _mediator.Send(command);
        return Ok(ApiResponse<ProductSpecificationDto>.SuccessResponse(spec, $"{EntityName} updated successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Xóa spec sản phẩm
    /// </summary>
    [HttpDelete("DeleteSpecification/{id}")]
    [Authorize(Roles = "Admin")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductSpecificationCommand { Id = id });
        return Ok(ApiResponse<int>.SuccessResponse(result, $"{EntityName} deleted successfully"));
    }
} 