using System;

namespace E_LaptopShop.Application.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int ReorderPoint { get; set; }
        public decimal AverageCost { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; } = null!; // Calculated field
        public decimal TotalValue { get; set; } // CurrentStock * AverageCost
        public bool NeedReorder { get; set; } // CurrentStock <= ReorderPoint
    }

    public class CreateInventoryDto
    {
        public int ProductId { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; } = 5;
        public int ReorderPoint { get; set; } = 10;
        public decimal AverageCost { get; set; }
        public string? Location { get; set; }
    }

    public class UpdateInventoryDto
    {
        public int MinimumStock { get; set; }
        public int ReorderPoint { get; set; }
        public decimal AverageCost { get; set; }
        public string? Location { get; set; }
    }

    public class StockAdjustmentDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = null!;
        public decimal UnitCost { get; set; }
        public string? Notes { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
    }
}