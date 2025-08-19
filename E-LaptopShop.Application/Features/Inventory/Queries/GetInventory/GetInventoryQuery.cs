using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetInventory
{
    public class GetInventoryQuery : IRequest<InventoryDto>
    {
        public int Id { get; set; }
    }
}
