using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

public class PriceHistoryDto
{
    public long LogId { get; set; }
    public DateTime ChangedAt { get; set; }
    public int? ChangedBy { get; set; }
    public string? Metadata { get; set; }
}

public class ProductsController : ApiV1ControllerBase
{
    private readonly IMediator _mediator;
    private readonly E_LaptopShop.Infra.ApplicationDbContext _db;
    public string EntityName = "Product";

    public ProductsController(IMediator mediator, E_LaptopShop.Infra.ApplicationDbContext db)
    {
        _mediator = mediator;
        _db       = db;
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
    /// 🔓 [PUBLIC] Lấy nhanh nhiều sản phẩm theo danh sách ID (dùng cho guest cart).
    /// Body: { ids: [1, 2, 3] }
    /// </summary>
    [HttpPost("batch")]
    [Tags(ApiTags.Public)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetBatch(
        [FromBody] BatchProductIdsRequest body)
    {
        if (body?.Ids == null || body.Ids.Count == 0)
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(System.Array.Empty<ProductDto>()));

        var list = new List<ProductDto>();
        foreach (var id in body.Ids.Distinct().Take(100))
        {
            try
            {
                var p = await _mediator.Send(new GetProductByIdQuery { Id = id });
                if (p != null) list.Add(p);
            }
            catch { /* skip id không tồn tại */ }
        }
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(list));
    }

    public class BatchProductIdsRequest
    {
        public List<int> Ids { get; set; } = new();
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

        var userId  = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var command = new UpdateProductCommand(requestDto, userId);
        var product = await _mediator.Send(command);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, $"{EntityName} updated successfully"));
    }

    /// <summary>
    /// 👑 [ADMIN] Lịch sử thay đổi giá — audit log.
    /// </summary>
    [HttpGet("GetPriceHistory/{id}")]
    [AdminOrManager]
    [Tags(ApiTags.Admin)]
    public async Task<ActionResult<ApiResponse<IEnumerable<PriceHistoryDto>>>> GetPriceHistory(int id)
    {
        var logs = await _db.ActivityLogs
            .Where(a => a.EventType == "PRICE_CHANGED" && a.Metadata!.Contains($"\"productId\":{id}"))
            .OrderByDescending(a => a.CreatedAt)
            .Take(50)
            .Select(a => new PriceHistoryDto
            {
                LogId     = a.Id,
                ChangedAt = a.CreatedAt,
                ChangedBy = a.UserId,
                Metadata  = a.Metadata,
            })
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<PriceHistoryDto>>.SuccessResponse(logs));
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