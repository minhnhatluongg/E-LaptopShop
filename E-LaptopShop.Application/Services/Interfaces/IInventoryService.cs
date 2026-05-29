using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto?> GetByProductIdAsync(int productId, CancellationToken ct = default);

        /// <summary>
        /// Trừ kho với Optimistic Locking.
        /// Retry tối đa maxRetries lần nếu gặp DbUpdateConcurrencyException.
        /// Trả về (true, newStock) nếu thành công, (false, errorMsg) nếu hết hàng/conflict.
        /// </summary>
        Task<(bool Success, int NewStock, string Message)> DeductStockAsync(
            int productId,
            int quantity,
            int? orderId     = null,
            string? reason   = null,
            int maxRetries   = 3,
            CancellationToken ct = default);

        /// <summary>Cộng kho (nhập hàng, hoàn trả).</summary>
        Task<(bool Success, int NewStock, string Message)> AddStockAsync(
            int productId,
            int quantity,
            int? orderId     = null,
            string? reason   = null,
            CancellationToken ct = default);

        /// <summary>Kiểm tra tồn kho trước khi đặt hàng (không trừ).</summary>
        Task<bool> IsAvailableAsync(int productId, int quantity, CancellationToken ct = default);
    }
}
