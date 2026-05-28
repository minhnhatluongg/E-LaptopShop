using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Features.Products.Commands.CreateProduct;
using E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;
using E_LaptopShop.Application.Features.Products.Commands.DeleteProduct;
using E_LaptopShop.Application.Features.Products.Queries.GetProductById;
using E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;
using E_LaptopShop.Application.Common.Pagination;

using E_LaptopShop.Helpers;
using E_LaptopShop.Controllers.Api_version;

namespace E_LaptopShop.Controllers;

public class ProductsController : ApiV1ControllerBase
{
    private readonly IMediator _mediator;
    public string EntityName = "Product";

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy tất cả sản phẩm - Dành cho catalog
    /// </summary>
    [HttpGet("GetAllProducts")]
    [Tags(ApiTags.Public)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(products));
    }

    /// <summary>
    /// 🔓 [PUBLIC] Lấy chi tiết sản phẩm - Dành cho catalog
    /// </summary>
    [HttpGet("GetProductById/{id}")]
    [Tags(ApiTags.Public)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
    }

    /// <summary>
    /// 👑 [ADMIN] Tạo sản phẩm mới
    /// </summary>
    [HttpPost("CreateProduct")]
    [AdminOrManager]
    [Tags(ApiTags.Admin)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequestDto requestDto)
    {
        var command = new CreateProductCommand(requestDto);
        var product = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ApiResponse<ProductDto>.SuccessResponse(product, $"{EntityName} created successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Cập nhật sản phẩm
    /// </summary>
    [HttpPut("UpdateProduct/{id}")]
    [AdminOrManager]
    [Tags(ApiTags.Admin)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductRequestDto requestDto)
    {
        if (id != requestDto.Id)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("ID mismatch"));
        }

        var command = new UpdateProductCommand(requestDto);
        var product = await _mediator.Send(command);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, $"{EntityName} updated successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Xóa sản phẩm
    /// </summary>
    [HttpDelete("DeleteProduct/{id}")]
    [AdminOnly]
    [Tags(ApiTags.Admin)]
    public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(new ApiResponse<int>
        {
            Success = true,
            Message = $"{EntityName} deleted successfully",
            Data = result
        });
    }
} 