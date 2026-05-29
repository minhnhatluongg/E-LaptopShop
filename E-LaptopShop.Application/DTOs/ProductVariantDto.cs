using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs
{
    // ─── Attribute / Value DTOs ────────────────────────────────────────────────

    public class ProductAttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<ProductAttributeValueDto> Values { get; set; } = new();
    }

    public class ProductAttributeValueDto
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class CreateAttributeDto
    {
        [Required] [MaxLength(100)] public string Name { get; set; } = null!;
        [Required] [MaxLength(100)] public string Slug { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }

    public class CreateAttributeValueDto
    {
        [Required] [MaxLength(100)] public string Value { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
    }

    // ─── Variant DTOs ─────────────────────────────────────────────────────────

    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Barcode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Danh sách giá trị attribute (RAM: 16GB, SSD: 512GB, ...).</summary>
        public List<ProductAttributeValueDto> Attributes { get; set; } = new();

        /// <summary>Nhãn tổng hợp để hiện trong UI: "16GB / 512GB".</summary>
        public string AttributeLabel =>
            string.Join(" / ", Attributes.OrderBy(a => a.AttributeName).Select(a => a.Value));
    }

    public class CreateVariantDto
    {
        [Required] public string SKU { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public string? Barcode { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> AttributeValueIds { get; set; } = new();
    }

    public class UpdateVariantDto
    {
        public string? SKU { get; set; }
        public decimal? Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int? StockQuantity { get; set; }
        public string? Barcode { get; set; }
        public bool? IsActive { get; set; }
    }

    // ─── Matrix Generator ─────────────────────────────────────────────────────

    /// <summary>
    /// Request để tự động sinh biến thể: chọn tổ hợp attribute value IDs.
    /// Ví dụ: ram=[1,2] x ssd=[3,4] → 4 biến thể.
    /// </summary>
    public class GenerateVariantMatrixDto
    {
        /// <summary>
        /// List các nhóm attribute value IDs. Mỗi phần tử = 1 trục biến thể.
        /// [[ramValue16Id, ramValue32Id], [ssd512Id, ssd1TBId]] → Cartesian product.
        /// </summary>
        public List<List<int>> AttributeGroups { get; set; } = new();

        /// <summary>Prefix cho auto-generated SKU. Ví dụ: "LAP-MAC-M3"</summary>
        public string? SkuPrefix { get; set; }

        /// <summary>Giá mặc định cho tất cả variants. Admin có thể sửa sau.</summary>
        public decimal DefaultPrice { get; set; }

        /// <summary>Số lượng kho mặc định.</summary>
        public int DefaultStock { get; set; } = 10;
    }

    /// <summary>Preview 1 variant trong matrix (trước khi confirm save).</summary>
    public class VariantPreviewDto
    {
        public string SKU { get; set; } = null!;
        public List<ProductAttributeValueDto> Attributes { get; set; } = new();
        public string AttributeLabel { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }

    /// <summary>Bulk save variants sau khi preview (admin đã điều chỉnh giá/stock).</summary>
    public class BulkSaveVariantsDto
    {
        public List<CreateVariantDto> Variants { get; set; } = new();
        public bool ClearExisting { get; set; } = false;
    }
}
