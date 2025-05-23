using Microsoft.AspNetCore.Mvc;
using MediatR;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage;
using E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage;
using E_LaptopShop.Application.Features.ProductImage.Commands.DeleteProductImgages;
using E_LaptopShop.Application.Features.ProductImage.Commands.SetMainImage;
using E_LaptopShop.Application.Features.ProductImage.Queries.GetAllFilteredAndPagination;
using E_LaptopShop.Application.Features.ProductImage.Queries.GetImagesByProductId;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Models;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductImageController> _logger;

        public ProductImageController(IMediator mediator, ILogger<ProductImageController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductImageDto>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? id = null,
            [FromQuery] int? productId = null,
            [FromQuery] string? imageUrl = null,
            [FromQuery] bool? isMain = null,
            [FromQuery] string? fileType = null,
            [FromQuery] long? fileSize = null,
            [FromQuery] int? displayOrder = null,
            [FromQuery] string? altText = null,
            [FromQuery] string? title = null,
            [FromQuery] DateTime? createdAt = null,
            [FromQuery] DateTime? uploadedAt = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isAscending = true,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string[]? searchFields = null)
        {
            try
            {
                var query = new GetAllFilteredAndPagitionQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Id = id,
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    IsMain = isMain,
                    FileType = fileType,
                    FileSize = fileSize,
                    DisplayOrder = displayOrder,
                    AltText = altText,
                    Title = title,
                    CreatedAt = createdAt,
                    UploadedAt = uploadedAt,
                    IsActive = isActive,
                    CreatedBy = createdBy,
                    SortBy = sortBy,
                    IsAscending = isAscending,
                    SearchTerm = searchTerm,
                    SearchFields = searchFields
                };

                var result = await _mediator.Send(query);
                return Ok(ApiResponse<PagedResult<ProductImageDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all product images");
                return StatusCode(500, ApiResponse<PagedResult<ProductImageDto>>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductImageDto>>>> GetByProductId(int productId)
        {
            try
            {
                var query = new GetImagesByProductIdQuery { ProductId = productId };
                var result = await _mediator.Send(query);
                return Ok(ApiResponse<IEnumerable<ProductImageDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting images for product {productId}");
                return StatusCode(500, ApiResponse<IEnumerable<ProductImageDto>>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductImageDto>>> Create([FromBody] CreateProductImageCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetByProductId), 
                    new { productId = result.ProductId }, 
                    ApiResponse<ProductImageDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product image");
                return StatusCode(500, ApiResponse<ProductImageDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductImageDto>>> Update(int id, [FromBody] UpdateProductImageCommand command)
        {
            try
            {
                if (id != command.Id)
                    return BadRequest(ApiResponse<ProductImageDto>.ErrorResponse("ID mismatch"));

                var result = await _mediator.Send(command);
                return Ok(ApiResponse<ProductImageDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating product image {id}");
                return StatusCode(500, ApiResponse<ProductImageDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
        {
            try
            {
                var command = new DeleteProductImageCommand(id);
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<int>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting product image {id}");
                return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred while processing your request"));
            }
        }

        [HttpPut("{id}/set-main")]
        public async Task<ActionResult<ApiResponse<ProductImageDto>>> SetMainImage(int id)
        {
            try
            {
                var command = new SetMainImageCommand(id);
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<ProductImageDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while setting main image {id}");
                return StatusCode(500, ApiResponse<ProductImageDto>.ErrorResponse("An error occurred while processing your request"));
            }
        }
    }
} 