using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Commands.AdjustStock
{
    public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, InventoryDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryHistoryRepository _historyRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public AdjustStockCommandHandler(
            IInventoryRepository inventoryRepository,
            IInventoryHistoryRepository historyRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _historyRepository = historyRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<InventoryDto> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Get or create inventory
            var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory not found for product ID {request.ProductId}");
            }

            var oldStock = inventory.CurrentStock;

            // Validate stock adjustment
            if (request.TransactionType == InventoryTransactionType.Sale && 
                inventory.CurrentStock < Math.Abs(request.Quantity))
            {
                throw new InvalidOperationException($"Insufficient stock. Available: {inventory.CurrentStock}, Requested: {Math.Abs(request.Quantity)}");
            }

            // Update stock
            var success = await _inventoryRepository.UpdateStockAsync(
                request.ProductId,
                request.Quantity,
                request.TransactionType);

            if (!success)
            {
                throw new InvalidOperationException("Failed to update inventory stock");
            }

            // Create history record
            var history = new Domain.Entities.InventoryHistory
            {
                InventoryId = inventory.Id,
                TransactionType = request.TransactionType.ToString(),
                Quantity = Math.Abs(request.Quantity), // Store positive quantity
                UnitCost = request.UnitCost,
                TransactionDate = DateTime.UtcNow,
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType,
                Notes = request.Notes ?? GenerateDefaultNotes(request.TransactionType, oldStock, request.Quantity),
                CreatedBy = request.CreatedBy
            };

            await _historyRepository.AddAsync(history);

            // Update average cost for purchase transactions
            if (request.TransactionType == InventoryTransactionType.Purchase)
            {
                await UpdateAverageCost(inventory, request.Quantity, request.UnitCost);
            }

            // Return updated inventory
            var updatedInventory = await _inventoryRepository.GetByIdAsync(inventory.Id);
            return _mapper.Map<InventoryDto>(updatedInventory);
        }

        private async Task UpdateAverageCost(Domain.Entities.Inventory inventory, int quantity, decimal unitCost)
        {
            // Calculate weighted average cost
            var totalValue = (inventory.CurrentStock * inventory.AverageCost) + (quantity * unitCost);
            var totalQuantity = inventory.CurrentStock + quantity;
            
            if (totalQuantity > 0)
            {
                inventory.AverageCost = totalValue / totalQuantity;
                inventory.LastPurchasePrice = unitCost;
                await _inventoryRepository.UpdateAsync(inventory);
            }
        }

        private string GenerateDefaultNotes(InventoryTransactionType transactionType, int oldStock, int quantity)
        {
            return transactionType switch
            {
                InventoryTransactionType.Purchase => $"Stock increased by {quantity} units",
                InventoryTransactionType.Sale => $"Stock decreased by {Math.Abs(quantity)} units (sale)",
                InventoryTransactionType.Return => $"Stock increased by {quantity} units (return)",
                InventoryTransactionType.Adjustment => $"Stock adjusted from {oldStock} units",
                InventoryTransactionType.Damaged => $"Stock decreased by {Math.Abs(quantity)} units (damaged goods)",
                InventoryTransactionType.Expired => $"Stock decreased by {Math.Abs(quantity)} units (expired goods)",
                InventoryTransactionType.Transfer => $"Stock transferred: {quantity} units",
                _ => $"Stock transaction: {transactionType}"
            };
        }
    }
}
