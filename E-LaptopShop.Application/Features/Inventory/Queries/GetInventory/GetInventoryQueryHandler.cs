using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetInventory
{
    public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, InventoryDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public GetInventoryQueryHandler(
            IInventoryRepository inventoryRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<InventoryDto> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {request.Id} not found");
            }

            return _mapper.Map<InventoryDto>(inventory);
        }
    }
}
