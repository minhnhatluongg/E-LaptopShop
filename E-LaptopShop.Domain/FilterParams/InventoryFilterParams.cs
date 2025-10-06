using E_LaptopShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.FilterParams
{
    public sealed class InventoryFilterParams
    {
        public int? Id { get; init; }
        public int? ProductId { get; init; }

        // Stock ranges
        public int? CurrentStockMin { get; init; }
        public int? CurrentStockMax { get; init; }
        public int? MinimumStockMin { get; init; }
        public int? MinimumStockMax { get; init; }
        public int? ReorderPointMin { get; init; }
        public int? ReorderPointMax { get; init; }

        // Cost ranges
        public decimal? AverageCostMin { get; init; }
        public decimal? AverageCostMax { get; init; }
        public decimal? LastPurchasePriceMin { get; init; }
        public decimal? LastPurchasePriceMax { get; init; }

        // Date ranges
        public DateTime? LastUpdatedFrom { get; init; }
        public DateTime? LastUpdatedTo { get; init; }

        public string? Location { get; init; }
        public InventoryStatus? Status { get; init; }

        // tuỳ chọn: có include navigation không
        public bool IncludeProduct { get; init; } = false;
    }
}