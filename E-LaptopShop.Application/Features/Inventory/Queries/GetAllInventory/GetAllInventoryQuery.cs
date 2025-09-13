using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory
{
    public class GetAllInventoryQuery : BasePagedQuery<InventoryDto>
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public int? CurrentStock { get; set; }
        public int? MinimumStock { get; set; }
        public int? ReorderPoint { get; set; }
        public decimal? AverageCost { get; set; }
        public decimal? LastPurchasePrice { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? Location { get; set; }
        public InventoryStatus? Status { get; set; }

        // Các flag dẫn xuất (không map trực tiếp cột)
        public bool? LowStockOnly { get; set; }      // CurrentStock <= MinimumStock
        public bool? OutOfStockOnly { get; set; }    // CurrentStock == 0
    }
}
