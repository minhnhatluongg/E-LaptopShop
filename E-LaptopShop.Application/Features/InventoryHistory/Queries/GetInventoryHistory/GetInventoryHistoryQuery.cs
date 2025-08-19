using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryHistory
{
    public class GetInventoryHistoryQuery : BasePagedQuery<InventoryHistoryDto>
    {
        // Specific filters for InventoryHistory
        public int? InventoryId { get; set; }
        public int? ProductId { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? CreatedBy { get; set; }
    }
}
