using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Interfaces
{
    /// <summary>
    /// Abstraction để Application layer push realtime event mà không phụ thuộc SignalR.
    /// Web layer cung cấp implementation cụ thể (SignalR Hub).
    /// Nếu không đăng ký service nào, các call trở thành no-op.
    /// </summary>
    public interface IInventoryNotifier
    {
        Task InventoryUpdatedAsync(int productId, int newStock, CancellationToken ct = default);

        Task BulkJobProgressAsync(string jobId, double progressPercent, int processedCount, int totalCount, CancellationToken ct = default);

        Task BulkJobCompletedAsync(string jobId, int success, int fail, string? jobType, CancellationToken ct = default);

        Task BulkJobFailedAsync(string jobId, string error, CancellationToken ct = default);
    }
}
