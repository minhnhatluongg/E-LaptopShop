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
using E_LaptopShop.Application.Features.Categories.Queries.GetFilterdPagedCategoriesQuery;
using E_LaptopShop.Application.Common.Pagination;

namespace E_LaptopShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public string EntityName => "Category";
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy tất cả danh mục - Dành cho catalog browsing
    /// </summary>
    [HttpGet("GetAllCategories")]
    [Tags("🔓 Public")]
    public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetAll(
        [FromQuery] GetAllCategoriesQuery request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return Ok(ApiResponse<PagedResult<CategoryDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy chi tiết danh mục - Dành cho catalog browsing
    /// </summary>
    [HttpGet("GetCategoryById/{id}")]
    [Tags("🔓 Public")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
    }

    /// <summary>
    /// 👑 [ADMIN] Tạo danh mục mới
    /// </summary>
    [HttpPost("CreateCategory")]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} created successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Cập nhật danh mục
    /// </summary>
    [HttpPut("UpdateCategory/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var category = await _mediator.Send(command);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} updated successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Xóa danh mục
    /// </summary>
    [HttpDelete("DeleteCategory/{id}")]
    [Authorize(Roles = "Admin")]
    [Tags("👑 Admin")]
    public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(new ApiResponse<int>
        {
            Success = true,
            Message = $"{EntityName} deleted successfully",
            Data = result
        });
    }

    [HttpGet("GetFilteredPagedCategories")]
    public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetFilteredPagedCategories(
        [FromQuery] GetFilteredPagedCategoriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<PagedResult<CategoryDto>>.SuccessResponse(result));
    }
} 