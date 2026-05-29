using System.Security.Claims;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1")]
    public class ProductVariantsController : ApiV1ControllerBase
    {
        private readonly IProductVariantRepository  _variantRepo;
        private readonly IProductAttributeRepository _attrRepo;
        private readonly ILogger<ProductVariantsController> _logger;

        public ProductVariantsController(
            IProductVariantRepository variantRepo,
            IProductAttributeRepository attrRepo,
            ILogger<ProductVariantsController> logger)
        {
            _variantRepo = variantRepo;
            _attrRepo    = attrRepo;
            _logger      = logger;
        }

        private int GetUserId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // ─── Attributes ──────────────────────────────────────────────────────

        /// <summary>🔓 [PUBLIC] Lấy tất cả attributes kèm values (RAM, SSD, Color, ...).</summary>
        [HttpGet("product-attributes")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductAttributeDto>>>> GetAttributes(
            CancellationToken ct)
        {
            var attrs = await _attrRepo.GetAllActiveWithValuesAsync(ct);
            var dtos  = attrs.Select(a => new ProductAttributeDto
            {
                Id = a.Id, Name = a.Name, Slug = a.Slug, IsActive = a.IsActive,
                Values = a.ProductAttributeValues.Select(v => new ProductAttributeValueDto
                {
                    Id = v.Id, AttributeId = a.Id, AttributeName = a.Name,
                    Value = v.Value, DisplayOrder = v.DisplayOrder,
                }).ToList(),
            });
            return Ok(ApiResponse<IEnumerable<ProductAttributeDto>>.SuccessResponse(dtos));
        }

        /// <summary>👑 [ADMIN] Tạo attribute mới (RAM, SSD, Color...).</summary>
        [HttpPost("product-attributes")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<ProductAttributeDto>>> CreateAttribute(
            [FromBody] CreateAttributeDto dto, CancellationToken ct)
        {
            try
            {
                var entity = new ProductAttribute { Name = dto.Name, Slug = dto.Slug, IsActive = dto.IsActive };
                var saved  = await _attrRepo.AddAsync(entity, ct);
                return Ok(ApiResponse<ProductAttributeDto>.SuccessResponse(
                    new ProductAttributeDto { Id = saved.Id, Name = saved.Name, Slug = saved.Slug, IsActive = saved.IsActive }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attribute");
                return BadRequest(ApiResponse<ProductAttributeDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👑 [ADMIN] Thêm giá trị vào attribute (ví dụ: thêm "32GB" vào attribute "RAM").</summary>
        [HttpPost("product-attributes/{attributeId:int}/values")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<ProductAttributeValueDto>>> AddValue(
            int attributeId, [FromBody] CreateAttributeValueDto dto, CancellationToken ct)
        {
            try
            {
                var maxOrder = (await _attrRepo.GetValueByIdAsync(0, ct)) != null ? 0 : 0; // just initialize
                var entity   = new ProductAttributeValue
                {
                    AttributeId  = attributeId,
                    Value        = dto.Value.Trim(),
                    DisplayOrder = dto.DisplayOrder,
                };
                var saved = await _attrRepo.AddValueAsync(entity, ct);
                return Ok(ApiResponse<ProductAttributeValueDto>.SuccessResponse(
                    new ProductAttributeValueDto
                    {
                        Id = saved.Id, AttributeId = attributeId,
                        Value = saved.Value, DisplayOrder = saved.DisplayOrder,
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attribute value");
                return BadRequest(ApiResponse<ProductAttributeValueDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👑 [ADMIN] Xoá giá trị attribute.</summary>
        [HttpDelete("product-attributes/values/{valueId:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<int>>> DeleteValue(int valueId, CancellationToken ct)
        {
            var ok = await _attrRepo.DeleteValueAsync(valueId, ct);
            return Ok(new ApiResponse<int> { Success = ok, Data = valueId, Message = ok ? "Deleted" : "Not found" });
        }

        // ─── Variants ────────────────────────────────────────────────────────

        /// <summary>🔓 [PUBLIC] Lấy tất cả variants của sản phẩm.</summary>
        [HttpGet("products/{productId:int}/variants")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductVariantDto>>>> GetByProduct(
            int productId, CancellationToken ct)
        {
            var variants = await _variantRepo.GetByProductIdAsync(productId, ct);
            var dtos     = variants.Select(MapToDto);
            return Ok(ApiResponse<IEnumerable<ProductVariantDto>>.SuccessResponse(dtos));
        }

        /// <summary>👑 [ADMIN] Xem preview matrix trước khi tạo (Cartesian product).</summary>
        [HttpPost("products/{productId:int}/variants/preview")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VariantPreviewDto>>>> PreviewMatrix(
            int productId, [FromBody] GenerateVariantMatrixDto dto, CancellationToken ct)
        {
            try
            {
                var previews = await GeneratePreviewsAsync(productId, dto, ct);
                return Ok(ApiResponse<IEnumerable<VariantPreviewDto>>.SuccessResponse(previews));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating variant preview");
                return BadRequest(ApiResponse<IEnumerable<VariantPreviewDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// 👑 [ADMIN] Tạo hàng loạt variants (bulk save).
        /// Dùng sau khi admin confirm preview + điều chỉnh giá/stock.
        /// </summary>
        [HttpPost("products/{productId:int}/variants/bulk")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductVariantDto>>>> BulkSave(
            int productId, [FromBody] BulkSaveVariantsDto dto, CancellationToken ct)
        {
            try
            {
                if (dto.ClearExisting)
                    await _variantRepo.DeleteByProductIdAsync(productId, ct);

                var created = new List<ProductVariant>();
                foreach (var v in dto.Variants)
                {
                    // Validate unique SKU
                    if (await _variantRepo.SKUExistsAsync(v.SKU, null, ct))
                        return BadRequest(ApiResponse<IEnumerable<ProductVariantDto>>.ErrorResponse(
                            $"SKU '{v.SKU}' đã tồn tại"));

                    var entity = new ProductVariant
                    {
                        ProductId     = productId,
                        SKU           = v.SKU,
                        Price         = v.Price,
                        CompareAtPrice = v.CompareAtPrice,
                        CostPrice     = v.CostPrice,
                        StockQuantity = v.StockQuantity,
                        Barcode       = v.Barcode,
                        IsActive      = v.IsActive,
                        CreatedAt     = DateTime.UtcNow,
                    };
                    created.Add(await _variantRepo.AddWithAttributesAsync(entity, v.AttributeValueIds, ct));
                }

                var dtos = created.Select(MapToDto);
                return Ok(ApiResponse<IEnumerable<ProductVariantDto>>.SuccessResponse(dtos,
                    $"Đã tạo {created.Count} biến thể thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk saving variants");
                return BadRequest(ApiResponse<IEnumerable<ProductVariantDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👑 [ADMIN] Cập nhật variant (giá, stock, SKU, ...).</summary>
        [HttpPut("variants/{id:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<ProductVariantDto>>> Update(
            int id, [FromBody] UpdateVariantDto dto, CancellationToken ct)
        {
            try
            {
                var variant = await _variantRepo.GetByIdAsync(id, ct);
                if (variant == null) return NotFound(ApiResponse<ProductVariantDto>.ErrorResponse("Không tìm thấy biến thể"));

                if (dto.SKU != null)
                {
                    if (await _variantRepo.SKUExistsAsync(dto.SKU, id, ct))
                        return BadRequest(ApiResponse<ProductVariantDto>.ErrorResponse($"SKU '{dto.SKU}' đã tồn tại"));
                    variant.SKU = dto.SKU;
                }
                if (dto.Price.HasValue)          variant.Price          = dto.Price.Value;
                if (dto.CompareAtPrice.HasValue)  variant.CompareAtPrice = dto.CompareAtPrice;
                if (dto.CostPrice.HasValue)       variant.CostPrice      = dto.CostPrice;
                if (dto.StockQuantity.HasValue)   variant.StockQuantity  = dto.StockQuantity.Value;
                if (dto.Barcode != null)          variant.Barcode        = dto.Barcode;
                if (dto.IsActive.HasValue)        variant.IsActive       = dto.IsActive.Value;

                await _variantRepo.UpdateAsync(variant, ct);
                return Ok(ApiResponse<ProductVariantDto>.SuccessResponse(MapToDto(variant)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating variant {Id}", id);
                return BadRequest(ApiResponse<ProductVariantDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👑 [ADMIN] Xoá một variant.</summary>
        [HttpDelete("variants/{id:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id, CancellationToken ct)
        {
            var ok = await _variantRepo.DeleteAsync(id, ct);
            return Ok(new ApiResponse<int> { Success = ok, Data = id, Message = ok ? "Deleted" : "Not found" });
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private async Task<List<VariantPreviewDto>> GeneratePreviewsAsync(
            int productId,
            GenerateVariantMatrixDto dto,
            CancellationToken ct)
        {
            // Resolve all AttributeValue entities
            var allIds     = dto.AttributeGroups.SelectMany(g => g).Distinct().ToList();
            var allValues  = new List<ProductAttributeValue>();
            foreach (var id in allIds)
            {
                var v = await _attrRepo.GetValueByIdAsync(id, ct);
                if (v != null) allValues.Add(v);
            }

            // Cartesian product of attribute groups
            var groups = dto.AttributeGroups
                .Select(group => group
                    .Select(id => allValues.FirstOrDefault(v => v.Id == id))
                    .Where(v => v != null)
                    .Select(v => v!)
                    .ToList())
                .Where(g => g.Count > 0)
                .ToList();

            var combinations = CartesianProduct(groups);

            var previews = new List<VariantPreviewDto>();
            int idx = 1;
            foreach (var combo in combinations)
            {
                var label   = string.Join(" / ", combo.Select(v => v.Value));
                var skuPart = string.Join("-", combo.Select(v => v.Value.Replace(" ", "").ToUpper()[..Math.Min(v.Value.Length, 4)]));
                var sku     = string.IsNullOrWhiteSpace(dto.SkuPrefix)
                    ? $"VAR-{productId}-{skuPart}-{idx:D2}"
                    : $"{dto.SkuPrefix}-{skuPart}";

                previews.Add(new VariantPreviewDto
                {
                    SKU            = sku,
                    Attributes     = combo.Select(v => new ProductAttributeValueDto
                    {
                        Id            = v.Id,
                        AttributeId   = v.AttributeId,
                        AttributeName = v.Attribute?.Name ?? "",
                        Value         = v.Value,
                        DisplayOrder  = v.DisplayOrder,
                    }).ToList(),
                    AttributeLabel    = label,
                    Price             = dto.DefaultPrice,
                    StockQuantity     = dto.DefaultStock,
                    AttributeValueIds = combo.Select(v => v.Id).ToList(),
                });
                idx++;
            }
            return previews;
        }

        private static IEnumerable<List<T>> CartesianProduct<T>(List<List<T>> groups)
        {
            IEnumerable<List<T>> result = new[] { new List<T>() };
            foreach (var group in groups)
            {
                result = result.SelectMany(
                    combo => group,
                    (combo, item) => combo.Concat(new[] { item }).ToList());
            }
            return result;
        }

        private static ProductVariantDto MapToDto(ProductVariant v) => new()
        {
            Id              = v.Id,
            ProductId       = v.ProductId,
            SKU             = v.SKU,
            Price           = v.Price,
            CompareAtPrice  = v.CompareAtPrice,
            CostPrice       = v.CostPrice,
            StockQuantity   = v.StockQuantity,
            Barcode         = v.Barcode,
            IsActive        = v.IsActive,
            CreatedAt       = v.CreatedAt,
            Attributes      = v.AttributeValue?.Select(av => new ProductAttributeValueDto
            {
                Id            = av.Id,
                AttributeId   = av.AttributeId,
                AttributeName = av.Attribute?.Name ?? "",
                Value         = av.Value,
                DisplayOrder  = av.DisplayOrder,
            }).ToList() ?? new(),
        };
    }
}
