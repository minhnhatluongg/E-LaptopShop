using E_LaptopShop.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory
{
    public class GetAllInventoryQuery : IRequest<IEnumerable<InventoryDto>>
    {
        public string? Location { get; set; }
        public bool? LowStockOnly { get; set; }
        public bool? OutOfStockOnly { get; set; }
    }
}
