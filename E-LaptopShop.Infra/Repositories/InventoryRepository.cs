using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(ApplicationDbContext context, ILogger<InventoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Inventory> AddAsync(Inventory inventory)
        {
            try
            {
                inventory.LastUpdated = DateTime.Now;
                await _context.Inventories.AddAsync(inventory);
                await _context.SaveChangesAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding inventory to the database.");
                throw new Exception("An error occurred while adding the inventory.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var intentory = await _context.Inventories.FindAsync(id);
                if (intentory == null) return false;
                _context.Inventories.Remove(intentory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory from the database.");
                throw new Exception("An error occurred while deleting the inventory.", ex);
            }
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            try
            {
                var inventories = await _context.Inventories
                    .Include(i => i.Product)
                    .ToListAsync();
                return inventories;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all inventories from the database.");
                throw new Exception("An error occurred while retrieving all inventories.", ex);
            }
        }


        public async Task<decimal> GetAverageCostAsync(int productId)
        {
            try
            {
                var inventory = await GetByProductIdAsync(productId);
                return inventory?.AverageCost ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average cost for product ID {ProductId}.", productId);
                throw new Exception("An error occurred while calculating the average cost.", ex);
            }
        }


        public Task<Inventory?> GetByIdAsync(int id)
        {
            try
            {
                var inventory = _context.Inventories
                    .Include(i => i.Product)
                    .Include(i => i.HistoryRecords)
                    .FirstOrDefaultAsync(i => i.Id == id);
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory by ID from the database.");
                throw new Exception("An error occurred while retrieving the inventory by ID.", ex);
            }
        }

        public async Task<IEnumerable<Inventory>> GetByLocationAsync(string location)
        {
            try
            {
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                    .Where(i => i.Location == location)
                    .ToListAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory by location from the database.");
                throw new Exception("An error occurred while retrieving the inventory by location.", ex);
            }
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            try
            {
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                    .FirstOrDefaultAsync(i => i.ProductId == productId);
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory by Product ID from the database.");
                throw new Exception("An error occurred while retrieving the inventory by Product ID.", ex);
            }
        }

        public async Task<IEnumerable<Inventory>> GetByStatusAsync(InventoryStatus status)
        {
            try
            {
                IQueryable<Inventory> query = _context.Inventories
                    .Include(i => i.Product);
                switch (status)
                {
                    case InventoryStatus.InStock:
                        query = query.Where(i => i.CurrentStock > i.MinimumStock);
                        break;
                    case InventoryStatus.LowStock:
                        query = query.Where(i => i.CurrentStock > 0 && i.CurrentStock <= i.MinimumStock);
                        break;
                    case InventoryStatus.OutOfStock:
                        query = query.Where(i => i.CurrentStock == 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), "Invalid inventory status");
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory by status from the database.");
                throw new Exception("An error occurred while retrieving the inventory by status.", ex);
            }
        }


        public async Task<IEnumerable<Inventory>> GetInventoryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    throw new ArgumentException("Start date cannot be later than end date.");
                }
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                    .Where(i => i.LastUpdated >= startDate && i.LastUpdated <= endDate)
                    .ToListAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory by date range from the database.");
                throw new Exception("An error occurred while retrieving the inventory by date range.", ex);
            }
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
        {
            try
            {
                var lowStockItems = await _context.Inventories
                    .Include(i => i.Product)
                    .Where(i => i.CurrentStock <= i.MinimumStock && i.CurrentStock > 0)
                    .ToListAsync();
                return lowStockItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock items from the database.");
                throw new Exception("An error occurred while retrieving low stock items.", ex);
            }
        }

        public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync()
        {
            try
            {
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                    .Where(i => i.CurrentStock == 0)
                    .ToListAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving out-of-stock items from the database.");
                throw new Exception("An error occurred while retrieving out-of-stock items.", ex);
            }
        }

        public async Task<int> GetTotalStockAsync()
        {
            try
            {
                var totalStock = await _context.Inventories.SumAsync(i => i.CurrentStock);
                return totalStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total stock from the database.");
                throw new Exception("An error occurred while retrieving the total stock.", ex);
            }
        }

        public async Task<bool> IsInStockAsync(int productId, int requestedQuantity)
        {
            try
            {
                var inventory = await GetByProductIdAsync(productId);
                return inventory != null && inventory.CurrentStock >= requestedQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock availability for product ID {ProductId}.", productId);
                throw new Exception("An error occurred while checking stock availability.", ex);
            }
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            if (inventory == null)
            {
                _logger.LogError("Attempted to update a null inventory object.");
                throw new ArgumentNullException(nameof(inventory), "Inventory cannot be null.");
            }
            try
            {
                inventory.LastUpdated = DateTime.UtcNow;
                var inventoryUp = _context.Inventories.Update(inventory);
                await _context.SaveChangesAsync();
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory in the database.");
                throw new Exception("An error occurred while updating the inventory.", ex);
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity, InventoryTransactionType transactionType)
        {
            try
            {
                var inventory = await GetByProductIdAsync(productId);
                if (inventory == null)
                {
                    _logger.LogWarning("Inventory for product ID {ProductId} not found.", productId);
                    return false;
                }
                switch(transactionType)
                {
                    case InventoryTransactionType.Purchase:
                    case InventoryTransactionType.Return:
                        inventory.CurrentStock += Math.Abs(quantity);
                        break;
                    case InventoryTransactionType.Sale:
                    case InventoryTransactionType.Damaged:
                    case InventoryTransactionType.Expired:
                        inventory.CurrentStock -= Math.Abs(quantity);
                        break;
                    case InventoryTransactionType.Adjustment:
                        // For adjustments, we can set the stock to the specified quantity
                        inventory.CurrentStock = quantity;
                        break;
                }
                inventory.CurrentStock = Math.Max(0, inventory.CurrentStock); // Ensure stock doesn't go negative
                await UpdateAsync(inventory);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product ID {ProductId}.", productId);
                throw new Exception("An error occurred while updating the stock.", ex);
            }
        }
    }
}
