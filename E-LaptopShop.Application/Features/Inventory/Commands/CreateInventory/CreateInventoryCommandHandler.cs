using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Commands.CreateInventory
{
    public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, InventoryDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CreateInventoryCommandHandler(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<InventoryDto> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Check if inventory already exists for this product
            var existingInventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
            if (existingInventory != null)
            {
                throw new InvalidOperationException($"Inventory already exists for product ID {request.ProductId}");
            }

            // Create inventory entity
            var inventory = new Domain.Entities.Inventory
            {
                ProductId = request.ProductId,
                CurrentStock = request.CurrentStock,
                MinimumStock = request.MinimumStock,
                ReorderPoint = request.ReorderPoint,
                AverageCost = request.AverageCost,
                LastPurchasePrice = request.AverageCost, // Initialize with average cost
                Location = request.Location,
                LastUpdated = DateTime.UtcNow
            };

            // Save inventory
            var createdInventory = await _inventoryRepository.AddAsync(inventory);
            
            // Load with product details and map to DTO
            var inventoryWithProduct = await _inventoryRepository.GetByIdAsync(createdInventory.Id);
            return _mapper.Map<InventoryDto>(inventoryWithProduct);
        }
    }
}