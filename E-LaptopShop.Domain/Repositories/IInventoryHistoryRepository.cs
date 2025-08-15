using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IInventoryHistoryRepository
    {
        // Basic CRUD operations

        Task<InventoryHistory?> GetByIdAsync(int id);
        Task<IEnumerable<InventoryHistory>> GetAllAsync();
        Task<IEnumerable<InventoryHistory>> GetByInventoryIdAsync(int productId);
        Task<InventoryHistory> AddAsync(InventoryHistory inventoryHistory);
        Task<bool> DeleteAsync(int id);

        // Business Logic
        Task<IEnumerable<InventoryHistory>> GetByTransactionTypeAsync(InventoryTransactionType transactionType);
        Task<IEnumerable<InventoryHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InventoryHistory>> GetByProductIdAsync(int productId);
        Task<IEnumerable<InventoryHistory>> GetByReferenceAsync(string referenceType, int referenceId);


        //Reporting 
        Task<IEnumerable<InventoryHistory>> GetTransactionHistoryAsync(int productId, DateTime? fromDate, DateTime? toDate);
        Task<decimal> GetTotalValueByTransactionTypeAsync(InventoryTransactionType transactionType, DateTime? fromDate, DateTime? toDate);
    }
}
