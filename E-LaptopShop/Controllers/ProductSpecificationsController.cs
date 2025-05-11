using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.CreateProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.DeleteProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Commands.UpdateProductSpecification;
using E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetAllProductSpecifications;
using E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetProductSpecificationById;
using E_LaptopShop.Application.Models;
using MediatR;
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

    [HttpGet("GetAllSpecifications")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductSpecificationDto>>>> GetAll([FromQuery] GetAllProductSpecificationsQuery query)
    {
        var specs = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<ProductSpecificationDto>>.SuccessResponse(specs));
    }

    [HttpGet("GetSpecificationById/{id}")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> GetById(int id)
    {
        var spec = await _mediator.Send(new GetProductSpecificationByIdQuery { Id = id });
        return Ok(ApiResponse<ProductSpecificationDto>.SuccessResponse(spec));
    }

    [HttpPost("CreateSpecification")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> Create([FromBody] CreateProductSpecificationCommand command)
    {
        var spec = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = spec.Id },
            ApiResponse<ProductSpecificationDto>.SuccessResponse(spec, $"{EntityName} created successfully"));
    }

    [HttpPut("UpdateSpecification/{id}")]
    public async Task<ActionResult<ApiResponse<ProductSpecificationDto>>> Update(int id, [FromBody] UpdateProductSpecificationCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<ProductSpecificationDto>.ErrorResponse("ID mismatch"));

        var spec = await _mediator.Send(command);
        return Ok(ApiResponse<ProductSpecificationDto>.SuccessResponse(spec, $"{EntityName} updated successfully"));
    }

    [HttpDelete("DeleteSpecification/{id}")]
    public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductSpecificationCommand { Id = id });
        return Ok(ApiResponse<int>.SuccessResponse(result, $"{EntityName} deleted successfully"));
    }
} 