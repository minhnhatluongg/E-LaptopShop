using E_LaptopShop.Application.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Inventory.Commands.UpdateInventory
{
    public class UpdateInventoryCommand : IRequest<InventoryDto>
    {
        public int Id { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReorderPoint { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal AverageCost { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }
    }
}
