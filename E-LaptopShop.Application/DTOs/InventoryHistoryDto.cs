using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using System;

namespace E_LaptopShop.Application.DTOs
{
    // ✨ FILTER DTO CHUẨN - kế thừa từ các base classes
    public class InventoryHistoryFilterDto : PaginationParams
    {
        // Specific filters
        public int? InventoryId { get; set; }
        public int? ProductId { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? CreatedBy { get; set; }
        
        // ✨ Search options
        public SearchOptions Search { get; set; } = new();
        
        // ✨ Sorting options  
        public SortingOptions Sort { get; set; } = new() 
        { 
            SortBy = "TransactionDate", 
            IsAscending = false 
        };
    }

    // Các DTOs khác giữ nguyên...
    public class InventoryHistoryDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public string TransactionTypeDisplay { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public string? ProductSku { get; set; }
        public string? Location { get; set; }
    }

    public class CreateInventoryHistoryDto
    {
        public int InventoryId { get; set; }
        public string TransactionType { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? Notes { get; set; }
    }

    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }
        public int InStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public DateTime ReportDate { get; set; }

        public decimal TotalPurchaseValue { get; set; }
        public decimal TotalSaleValue { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class UpdateInventoryHistoryDto
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
    }

    public class TransactionSummaryDto
    {
        public string TransactionType { get; set; } = null!;
        public string TransactionTypeDisplay { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int TransactionCount { get; set; }
    }
}