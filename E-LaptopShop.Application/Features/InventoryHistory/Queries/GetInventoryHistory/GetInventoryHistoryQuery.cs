using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryHistory
{
    public class GetInventoryHistoryQuery : BasePagedQuery<InventoryHistoryDto>
    {
        public int? InventoryId { get; set; }
        public int? ProductId { get; set; }
        public string? TransactionType { get; set; }  // Sale, Purchase, Adjustment, etc.
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? CreatedBy { get; set; }

        public string? ReferenceType { get; set; }    // "Order", "Adjustment", "Return"
        public int? ReferenceId { get; set; }         // OrderId, AdjustmentId, etc.

        public decimal? MinValue { get; set; }        // MinValue = Quantity * UnitCost
        public decimal? MaxValue { get; set; }        // MaxValue = Quantity * UnitCost
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }

        public int? CategoryId { get; set; }          // Filter by product category
        public string? ProductName { get; set; }      // Search by product name

        public string? Location { get; set; }         // Warehouse/Store location
        public bool? HasNotes { get; set; }           // Filter records with/without notes
    }
}
