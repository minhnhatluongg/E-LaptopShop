using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Features.Categories.Commands.CreateCategory;
using E_LaptopShop.Application.Features.Categories.Commands.UpdateCategory;
using E_LaptopShop.Application.Features.Categories.Commands.DeleteCategory;
using E_LaptopShop.Application.Features.Categories.Queries.GetCategoryById;
using E_LaptopShop.Application.Features.Categories.Queries.GetAllCategories;
using E_LaptopShop.Application.Common.Pagination;

namespace E_LaptopShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private const string EntityName = "Category";

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy tất cả danh mục (có hỗ trợ lọc và phân trang)
    /// </summary>
    [HttpGet]
    [Tags("🔓 Public")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetAll(
        [FromQuery] GetAllCategoriesQuery query,
        CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<CategoryDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy chi tiết danh mục theo ID
    /// </summary>
    [HttpGet("{id:int}")]
    [Tags("🔓 Public")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id }, ct);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(result));
    }

    /// <summary>
    /// 👑 [ADMIN] Tạo danh mục mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken ct)
    {
        var category = await _mediator.Send(command, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} created successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Cập nhật danh mục
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(
        int id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken ct)
    {
        command.Id = id;
        var category = await _mediator.Send(command, ct);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} updated successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Xóa danh mục
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [Tags("👑 Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand { Id = id }, ct);
        return Ok(ApiResponse<bool>.SuccessResponse(result, $"{EntityName} deleted successfully"));
    }
}
