using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Features.Products.Commands.CreateProduct;
using E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;
using E_LaptopShop.Application.Features.Products.Commands.DeleteProduct;
using E_LaptopShop.Application.Features.Products.Queries.GetProductById;
using E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;

namespace E_LaptopShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public string EntityName = "Product";

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("GetAllProducts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products));
    }

    [HttpGet("GetProductById/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
    }

    [HttpPost("CreateProduct")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ApiResponse<ProductDto>.SuccessResponse(product, $"{EntityName} created successfully"));
    }

    [HttpPut("UpdateProduct/{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("ID mismatch"));
        }

        var product = await _mediator.Send(command);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, $"{EntityName} updated successfully"));
    }

    [HttpDelete("DeleteProduct/{id}")]
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