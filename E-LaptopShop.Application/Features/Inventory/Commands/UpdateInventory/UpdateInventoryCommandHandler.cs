using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Commands.UpdateInventory
{
    public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, InventoryDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public UpdateInventoryCommandHandler(
            IInventoryRepository inventoryRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<InventoryDto> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {request.Id} not found");
            }

            // Update inventory properties
            inventory.MinimumStock = request.MinimumStock;
            inventory.ReorderPoint = request.ReorderPoint;
            inventory.AverageCost = request.AverageCost;
            inventory.Location = request.Location;
            inventory.LastUpdated = DateTime.UtcNow;

            // Save changes
            var updatedInventory = await _inventoryRepository.UpdateAsync(inventory);
            
            return _mapper.Map<InventoryDto>(updatedInventory);
        }
    }
}
