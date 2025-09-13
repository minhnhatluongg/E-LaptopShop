using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public IQueryable<Inventory> GetFilteredQueryable(
            InventoryFilterParams f)
        {
            var q = _context.Inventories.AsQueryable();

            // include khi thực sự cần object graph (nếu dùng ProjectTo<TDto> thường KHÔNG cần Include)
            if (f.IncludeProduct)
                q = q.Include(i => i.Product).AsSplitQuery();

            // ---- Exact filters ----
            if (f.Id.HasValue) q = q.Where(i => i.Id == f.Id.Value);
            if (f.ProductId.HasValue) q = q.Where(i => i.ProductId == f.ProductId.Value);

            if (!string.IsNullOrWhiteSpace(f.Location))
            {
                var loc = f.Location.Trim();
                q = q.Where(i => i.Location == loc); // SQL Server mặc định case-insensitive; nếu cần: EF.Functions.Like(i.Location, loc)
            }

            // normalize + apply ranges (dùng biến cục bộ)
            var (csMin, csMax) = Normalize(f.CurrentStockMin, f.CurrentStockMax);
            var (msMin, msMax) = Normalize(f.MinimumStockMin, f.MinimumStockMax);
            var (rpMin, rpMax) = Normalize(f.ReorderPointMin, f.ReorderPointMax);
            var (acMin, acMax) = Normalize(f.AverageCostMin, f.AverageCostMax);
            var (lpMin, lpMax) = Normalize(f.LastPurchasePriceMin, f.LastPurchasePriceMax);
            var (luFrom, luTo) = Normalize(f.LastUpdatedFrom, f.LastUpdatedTo);

            if (csMin.HasValue) q = q.Where(i => i.CurrentStock >= csMin.Value);
            if (csMax.HasValue) q = q.Where(i => i.CurrentStock <= csMax.Value);
            if (msMin.HasValue) q = q.Where(i => i.MinimumStock >= msMin.Value);
            if (msMax.HasValue) q = q.Where(i => i.MinimumStock <= msMax.Value);
            if (rpMin.HasValue) q = q.Where(i => i.ReorderPoint >= rpMin.Value);
            if (rpMax.HasValue) q = q.Where(i => i.ReorderPoint <= rpMax.Value);
            if (acMin.HasValue) q = q.Where(i => i.AverageCost >= acMin.Value);
            if (acMax.HasValue) q = q.Where(i => i.AverageCost <= acMax.Value);
            if (lpMin.HasValue) q = q.Where(i => i.LastPurchasePrice >= lpMin.Value);
            if (lpMax.HasValue) q = q.Where(i => i.LastPurchasePrice <= lpMax.Value);
            if (luFrom.HasValue) q = q.Where(i => i.LastUpdated >= luFrom.Value);
            if (luTo.HasValue) q = q.Where(i => i.LastUpdated <= luTo.Value);

            // ---- Derived Status (nghiệp vụ) ----
            if (f.Status.HasValue)
            {
                q = f.Status.Value switch
                {
                    InventoryStatus.InStock => q.Where(i => i.CurrentStock > i.MinimumStock),
                    InventoryStatus.LowStock => q.Where(i => i.CurrentStock > 0 && i.CurrentStock <= i.MinimumStock),
                    InventoryStatus.OutOfStock => q.Where(i => i.CurrentStock == 0),
                    InventoryStatus.Reordering => q.Where(i => i.CurrentStock <= i.ReorderPoint),
                    InventoryStatus.Discontinued => q.Where(i => i.CurrentStock == 0 && i.MinimumStock == 0),
                    _ => q
                };
            }

            return q.AsNoTracking();
        }

        #region Helper methods for normalization
        private static (int? min, int? max) Normalize(int? min, int? max)
        {
            if (min.HasValue && max.HasValue && min > max)
                (min, max) = (max, min);
            return (min, max);
        }

        private static (decimal? min, decimal? max) Normalize(decimal? min, decimal? max)
        {
            if (min.HasValue && max.HasValue && min > max)
                (min, max) = (max, min);
            return (min, max);
        }

        private static (DateTime? from, DateTime? to) Normalize(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue && from > to)
                (from, to) = (to, from);
            return (from, to);
        }

        #endregion

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

        public IQueryable<Inventory> GetQueryable()
        {
            return _context.Inventories
            .Include(i => i.Product)        
                .ThenInclude(p => p.Category)  
            .AsQueryable();
        }

        public IQueryable<Inventory> GetFilteredQueryable(int? id = null,
            int? productId = null,
            int? currentStock = null,
            int? minimumStock = null,
            int? reorderPoint = null,
            decimal? averageCost = null,
            decimal? lastPurchasePrice = null,
            DateTime? lastUpdated = null,
            string? location = null,
            InventoryStatus? status = null)
        {
            var q = _context.Inventories
                    .AsNoTracking()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.ProductSpecifications)
                    .AsQueryable();

            if (id.HasValue) q = q.Where(i => i.Id == id.Value);
            if (productId.HasValue) q = q.Where(i => i.ProductId == productId.Value);
            if (currentStock.HasValue) q = q.Where(i => i.CurrentStock == currentStock.Value);
            if (minimumStock.HasValue) q = q.Where(i => i.MinimumStock == minimumStock.Value);
            if (reorderPoint.HasValue) q = q.Where(i => i.ReorderPoint == reorderPoint.Value);
            if (averageCost.HasValue) q = q.Where(i => i.AverageCost == averageCost.Value);
            if (lastPurchasePrice.HasValue) q = q.Where(i => i.LastPurchasePrice == lastPurchasePrice.Value);
            if (lastUpdated.HasValue) q = q.Where(i => i.LastUpdated.Date == lastUpdated.Value.Date);
            if (!string.IsNullOrWhiteSpace(location)) q = q.Where(i => i.Location == location);
            if (status.HasValue) q = q.Where(i => i.Status == status.Value);

            return q;
        }
    }
}
