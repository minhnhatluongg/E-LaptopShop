using System;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Application.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace E_LaptopShop.Hubs
{
    /// <summary>
    /// Implementation IInventoryNotifier dùng SignalR Hub.
    /// Đặt ở Web layer để Application không phải reference SignalR/Web project.
    /// </summary>
    public class SignalRInventoryNotifier : IInventoryNotifier
    {
        private readonly IHubContext<InventoryHub> _hub;

        public SignalRInventoryNotifier(IHubContext<InventoryHub> hub)
        {
            _hub = hub;
        }

        public Task InventoryUpdatedAsync(int productId, int newStock, CancellationToken ct = default)
            => _hub.Clients.Group("admin").SendAsync(
                "InventoryUpdated",
                new { productId, newStock, updatedAt = DateTime.UtcNow },
                ct);

        public Task BulkJobProgressAsync(string jobId, double progressPercent, int processedCount, int totalCount, CancellationToken ct = default)
            => _hub.Clients.Group("admin").SendAsync(
                "BulkJobProgress",
                new { jobId, progressPercent, processedCount, totalCount },
                ct);

        public Task BulkJobCompletedAsync(string jobId, int success, int fail, string? jobType, CancellationToken ct = default)
            => _hub.Clients.Group("admin").SendAsync(
                "BulkJobCompleted",
                new
                {
                    jobId,
                    success,
                    fail,
                    message = fail == 0
                        ? $"✅ Xử lý xong {success} sản phẩm"
                        : $"⚠️ {success} thành công, {fail} lỗi",
                    jobType,
                },
                ct);

        public Task BulkJobFailedAsync(string jobId, string error, CancellationToken ct = default)
            => _hub.Clients.Group("admin").SendAsync(
                "BulkJobFailed",
                new { jobId, error },
                ct);
    }
}
