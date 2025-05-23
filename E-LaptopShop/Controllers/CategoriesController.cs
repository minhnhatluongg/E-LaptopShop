using System.Collections.Generic;
using System.Threading.Tasks;
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

    [HttpGet("GetAllCategories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories));
    }

    [HttpGet("GetCategoryById/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
    }

    [HttpPost("CreateCategory")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} created successfully"));
    }

    [HttpPut("UpdateCategory/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var category = await _mediator.Send(command);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, $"{EntityName} updated successfully"));
    }

    [HttpDelete("DeleteCategory/{id}")]
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