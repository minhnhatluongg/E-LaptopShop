using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Inventory.Commands.AdjustStock
{
    public class AdjustStockCommand : IRequest<InventoryDto>
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public InventoryTransactionType TransactionType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitCost { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        // Internal property set by controller
        public string CreatedBy { get; set; } = string.Empty;
    }
}
