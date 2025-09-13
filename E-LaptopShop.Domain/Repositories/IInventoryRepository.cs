using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IInventoryRepository
    {
        //Basic CRUD operations
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<IEnumerable<Inventory>> GetByLocationAsync(string location);
        Task<Inventory> AddAsync(Inventory inventory);
        Task<Inventory> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int id);

        IQueryable<Inventory> GetQueryable();

        IQueryable<Inventory> GetFilteredQueryable(int? id = null,
            int? productId = null,
            int? currentStock = null,
            int? minimumStock = null,
            int? reorderPoint = null,
            decimal? averageCost = null,
            decimal? lastPurchasePrice = null,
            DateTime? lastUpdated = null,
            string? location = null,
            InventoryStatus? status = null); 
        //Business Logic

        Task<bool> UpdateStockAsync(int productId, int quantity, InventoryTransactionType transactionType);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync();
        Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync();
        Task<IEnumerable<Inventory>> GetByStatusAsync(InventoryStatus status);

        Task<bool> IsInStockAsync(int productId, int requestedQuantity);
        Task<decimal> GetAverageCostAsync(int productId);

        //Reporting
        Task<IEnumerable<Inventory>> GetInventoryByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalStockAsync();

    }
}
