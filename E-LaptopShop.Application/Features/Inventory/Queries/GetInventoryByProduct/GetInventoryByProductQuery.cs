using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetInventoryByProduct
{
    public class GetInventoryByProductQuery : IRequest<InventoryDto>
    {
        public int ProductId { get; set; }
    }
}
