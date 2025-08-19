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
    public class InventoryHistoryRepository : IInventoryHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryHistoryRepository> _logger;
        

        public async Task<InventoryHistory> AddAsync(InventoryHistory inventoryHistory)
        {
            try
            {
                if (inventoryHistory == null)
                    throw new ArgumentNullException(nameof(inventoryHistory), "Inventory history cannot be null");
                inventoryHistory.TransactionDate = DateTime.UtcNow;
                await _context.InventoryHistories.AddAsync(inventoryHistory);
                await _context.SaveChangesAsync();
                return inventoryHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory history to database");
                throw new InvalidOperationException("An error occurred while adding inventory history.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var history = await _context.InventoryHistories.FindAsync(id);
                if (history == null)
                    throw new KeyNotFoundException($"Inventory history with ID {id} not found.");
                _context.InventoryHistories.Remove(history);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory history with ID {Id}", id);
                throw new InvalidOperationException("An error occurred while deleting inventory history.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetAllAsync()
        {
            try
            {
                var inventoryHistory = await _context.InventoryHistories
                    .Include(i => i.Inventory)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(i => i.TransactionDate)
                    .ToListAsync();
                return inventoryHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all inventory histories");
                throw new Exception("An error occurred while retrieving inventory histories.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    throw new ArgumentException("Start date cannot be later than end date");
                }

                return await _context.InventoryHistories
                    .Include(h => h.Inventory)
                        .ThenInclude(i => i.Product)
                    .Where(h => h.TransactionDate >= startDate && h.TransactionDate <= endDate)
                    .OrderByDescending(h => h.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by date range {StartDate} - {EndDate}", startDate, endDate);
                throw new Exception("An error occurred while retrieving inventory history by date range.", ex);
            }
        }

        public async Task<InventoryHistory?> GetByIdAsync(int id)
        {
            try
            {
                var inventoryHistory = _context.InventoryHistories
                    .Include(i => i.Inventory)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault(i => i.Id == id);
                if (inventoryHistory == null)
                    throw new KeyNotFoundException($"Inventory history with ID {id} not found.");
                return inventoryHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by ID {Id}", id);
                throw new InvalidOperationException("An error occurred while retrieving inventory history.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetByInventoryIdAsync(int productId)
        {
            try
            {
                var inventoryHistories = await _context.InventoryHistories
                    .Where(i => i.InventoryId == productId)
                    .Include(i => i.Inventory)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(i => i.TransactionDate)
                    .ToListAsync();
                return inventoryHistories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by product ID {ProductId}", productId);
                throw new InvalidOperationException("An error occurred while retrieving inventory history by product ID.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetByProductIdAsync(int productId)
        {
            try
            {
                return await _context.InventoryHistories
                    .Include(h => h.Inventory)
                        .ThenInclude(i => i.Product)
                    .Where(h => h.Inventory.ProductId == productId)
                    .OrderByDescending(h => h.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by product ID {ProductId}", productId);
                throw new Exception("An error occurred while retrieving inventory history by product ID.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetByReferenceAsync(string referenceType, int referenceId)
        {
            try
            {
                return await _context.InventoryHistories
                    .Include(h => h.Inventory)
                        .ThenInclude(i => i.Product)
                    .Where(h => h.ReferenceType == referenceType && h.ReferenceId == referenceId)
                    .OrderByDescending(h => h.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by reference type {ReferenceType} and ID {ReferenceId}", referenceType, referenceId);
                throw new InvalidOperationException("An error occurred while retrieving inventory history by reference.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetByTransactionTypeAsync(InventoryTransactionType transactionType)
        {
            try
            {
                var inventoryHistory = await _context.InventoryHistories
                    .Where(i => i.TransactionType == transactionType.ToString())
                    .Include(i => i.Inventory)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(i => i.TransactionDate)
                    .ToListAsync();
                return inventoryHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory history by transaction type {TransactionType}", transactionType);
                throw new InvalidOperationException("An error occurred while retrieving inventory history by transaction type.", ex);
            }
        }

        public async Task<decimal> GetTotalValueByTransactionTypeAsync(InventoryTransactionType transactionType, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = _context.InventoryHistories
                    .Where(h => h.TransactionType == transactionType.ToString());

                if (fromDate.HasValue)
                {
                    query = query.Where(h => h.TransactionDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(h => h.TransactionDate <= toDate.Value);
                }

                return await query.SumAsync(h => h.Quantity * h.UnitCost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total value for transaction type {TransactionType}", transactionType);
                throw new Exception("An error occurred while calculating total value.", ex);
            }
        }

        public async Task<IEnumerable<InventoryHistory>> GetTransactionHistoryAsync(int productId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = _context.InventoryHistories
                    .Include(h => h.Inventory)
                        .ThenInclude(i => i.Product)
                    .Where(h => h.Inventory.ProductId == productId);

                if (fromDate.HasValue)
                {
                    query = query.Where(h => h.TransactionDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(h => h.TransactionDate <= toDate.Value);
                }

                return await query
                    .OrderByDescending(h => h.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction history for product ID {ProductId}", productId);
                throw new Exception("An error occurred while retrieving transaction history.", ex);
            }
        }
    }
}
