using E_LaptopShop.Application.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.Features.Inventory.Commands.CreateInventory
{
    public class CreateInventoryCommand : IRequest<InventoryDto>
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MinimumStock { get; set; } = 5;

        [Required]
        [Range(0, int.MaxValue)]
        public int ReorderPoint { get; set; } = 10;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal AverageCost { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }
    }
}