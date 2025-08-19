using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory
{
    public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, IEnumerable<InventoryDto>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public GetAllInventoryQueryHandler(
            IInventoryRepository inventoryRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryDto>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Inventory> inventories;

            if (request.LowStockOnly == true)
            {
                inventories = await _inventoryRepository.GetLowStockItemsAsync();
            }
            else if (request.OutOfStockOnly == true)
            {
                inventories = await _inventoryRepository.GetOutOfStockItemsAsync();
            }
            else if (!string.IsNullOrEmpty(request.Location))
            {
                inventories = await _inventoryRepository.GetByLocationAsync(request.Location);
            }
            else
            {
                inventories = await _inventoryRepository.GetAllAsync();
            }

            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }
    }
}
