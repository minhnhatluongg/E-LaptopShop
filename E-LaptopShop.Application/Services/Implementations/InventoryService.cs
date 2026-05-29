using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository     _inventoryRepo;
        private readonly IInventoryHistoryRepository _historyRepo;
        private readonly IMapper                  _mapper;
        private readonly ILogger<InventoryService> _logger;

        // Realtime notifier — optional, nullable nếu chưa đăng ký
        private readonly IInventoryNotifier? _notifier;

        public InventoryService(
            IInventoryRepository inventoryRepo,
            IInventoryHistoryRepository historyRepo,
            IMapper mapper,
            ILogger<InventoryService> logger,
            IInventoryNotifier? notifier = null)
        {
            _inventoryRepo = inventoryRepo;
            _historyRepo   = historyRepo;
            _mapper        = mapper;
            _logger        = logger;
            _notifier      = notifier;
        }

        public async Task<InventoryDto?> GetByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var inv = await _inventoryRepo.GetByProductIdAsync(productId);
            return inv == null ? null : _mapper.Map<InventoryDto>(inv);
        }

        public async Task<bool> IsAvailableAsync(int productId, int quantity, CancellationToken ct = default)
        {
            var inv = await _inventoryRepo.GetByProductIdAsync(productId);
            return inv != null && inv.CurrentStock >= quantity;
        }

        /// <summary>
        /// Trừ kho với Optimistic Locking (RowVersion / EF Core concurrency token).
        ///
        /// Flow:
        ///   1. Đọc Inventory (kể cả RowVersion)
        ///   2. Kiểm tra CurrentStock >= quantity
        ///   3. Ghi UPDATE — EF sinh: WHERE Id=@id AND RowVersion=@rv
        ///   4. Nếu một request khác đã update trước → RowVersion thay đổi
        ///      → DbUpdateConcurrencyException
        ///   5. Reload entity và retry (tối đa maxRetries lần)
        ///   6. Sau maxRetries vẫn conflict → thông báo "Hàng đang có người mua"
        /// </summary>
        public async Task<(bool Success, int NewStock, string Message)> DeductStockAsync(
            int productId, int quantity,
            int? orderId = null, string? reason = null,
            int maxRetries = 3, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "[Inventory] DeductStock start — ProductId={ProductId} Qty={Qty} OrderId={OrderId}",
                productId, quantity, orderId);

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var inventory = await _inventoryRepo.GetByProductIdAsync(productId);

                if (inventory == null)
                    return (false, 0, "Không tìm thấy tồn kho sản phẩm");

                if (inventory.CurrentStock < quantity)
                {
                    _logger.LogWarning(
                        "[Inventory] Insufficient stock — ProductId={ProductId} Have={Have} Need={Need}",
                        productId, inventory.CurrentStock, quantity);
                    return (false, inventory.CurrentStock,
                        $"Sản phẩm vừa hết hàng — còn lại {inventory.CurrentStock} sản phẩm");
                }

                var prevStock = inventory.CurrentStock;
                inventory.CurrentStock -= quantity;
                inventory.LastUpdated   = DateTime.UtcNow;
                inventory.Status        = inventory.CurrentStock <= 0
                    ? InventoryStatus.OutOfStock
                    : inventory.CurrentStock <= inventory.MinimumStock
                        ? InventoryStatus.LowStock
                        : InventoryStatus.InStock;

                try
                {
                    await _inventoryRepo.UpdateAsync(inventory, ct);

                    _logger.LogInformation(
                        "[Inventory] Deducted — ProductId={ProductId} {Prev}→{New} Attempt={Attempt}",
                        productId, prevStock, inventory.CurrentStock, attempt);

                    // Ghi history
                    await RecordHistoryAsync(inventory.Id, productId, orderId,
                        InventoryTransactionType.Sale, -quantity,
                        reason ?? $"Đặt hàng #{orderId}", prevStock, inventory.CurrentStock, ct);

                    // Đẩy SignalR notification lên admin dashboard
                    await PushStockUpdateAsync(productId, inventory.CurrentStock,
                        inventory.ProductId, ct);

                    return (true, inventory.CurrentStock, "OK");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(
                        "[Inventory] Concurrency conflict — ProductId={ProductId} Attempt={Attempt}/{Max}",
                        productId, attempt, maxRetries);

                    // Reload entity để lấy giá trị mới nhất từ DB
                    foreach (var entry in ex.Entries)
                        await entry.ReloadAsync(ct);
                }
            }

            _logger.LogError(
                "[Inventory] Failed after {Max} retries — ProductId={ProductId}",
                maxRetries, productId);

            return (false, 0,
                "Hàng đang có người mua cùng lúc — vui lòng thử lại sau vài giây");
        }

        public async Task<(bool Success, int NewStock, string Message)> AddStockAsync(
            int productId, int quantity,
            int? orderId = null, string? reason = null,
            CancellationToken ct = default)
        {
            var inventory = await _inventoryRepo.GetByProductIdAsync(productId);
            if (inventory == null)
                return (false, 0, "Không tìm thấy tồn kho");

            var prevStock = inventory.CurrentStock;
            inventory.CurrentStock += quantity;
            inventory.LastUpdated   = DateTime.UtcNow;
            inventory.Status        = InventoryStatus.InStock;

            await _inventoryRepo.UpdateAsync(inventory, ct);

            await RecordHistoryAsync(inventory.Id, productId, orderId,
                InventoryTransactionType.Purchase, quantity,
                reason ?? "Nhập kho", prevStock, inventory.CurrentStock, ct);

            await PushStockUpdateAsync(productId, inventory.CurrentStock, productId, ct);

            return (true, inventory.CurrentStock, "OK");
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task RecordHistoryAsync(
            int inventoryId, int productId, int? orderId,
            InventoryTransactionType txType, int quantity,
            string note, int stockBefore, int stockAfter,
            CancellationToken ct)
        {
            try
            {
                var history = new InventoryHistory
                {
                    InventoryId     = inventoryId,
                    TransactionType = txType.ToString(),   // stored as string in DB
                    Quantity        = Math.Abs(quantity),
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId     = orderId,
                    ReferenceType   = orderId.HasValue ? "Order" : null,
                    Notes           = note,
                    CreatedBy       = "System",
                };
                await _historyRepo.AddAsync(history, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Inventory] Failed to record history for product {ProductId}", productId);
            }
        }

        private async Task PushStockUpdateAsync(int productId, int newStock,
            int _productId, CancellationToken ct)
        {
            if (_notifier == null) return;
            try
            {
                await _notifier.InventoryUpdatedAsync(productId, newStock, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[SignalR] Failed to push inventory update for product {ProductId}", productId);
            }
        }
    }

    /// <summary>Payload gửi qua SignalR khi kho thay đổi.</summary>
    public class InventoryUpdatePayload
    {
        public int ProductId { get; set; }
        public int NewStock { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
