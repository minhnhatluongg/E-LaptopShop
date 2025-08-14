using E_LaptopShop.Domain.Enums;

namespace E_LaptopShop.Application.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductSKU { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public OrderItemStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? SerialNumber { get; set; }
        public string? Notes { get; set; }
        
        // Calculated fields
        public decimal TotalPrice => (UnitPrice * Quantity) - DiscountAmount + TaxAmount;
        public decimal OriginalPrice => UnitPrice * Quantity;
    }
}
