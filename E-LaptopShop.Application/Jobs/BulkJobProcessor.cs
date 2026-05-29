using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Jobs
{
    /// <summary>
    /// BackgroundService — chạy song song với HTTP pipeline.
    /// Đọc job từ Channel, xử lý theo batch 50, cập nhật Registry + push SignalR.
    ///
    /// Pattern: long-running hosted service (start khi app start, stop khi app stop).
    /// </summary>
    public class BulkJobProcessor : BackgroundService
    {
        private readonly IBulkJobQueue     _queue;
        private readonly BulkJobRegistry   _registry;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BulkJobProcessor> _logger;

        // Realtime notifier injected dynamically để tránh circular dependency
        private IInventoryNotifier? _notifier;

        private const int BatchSize = 50; // số bản ghi update mỗi lần SaveChanges

        public BulkJobProcessor(
            IBulkJobQueue queue,
            BulkJobRegistry registry,
            IServiceScopeFactory scopeFactory,
            ILogger<BulkJobProcessor> logger)
        {
            _queue       = queue;
            _registry    = registry;
            _scopeFactory = scopeFactory;
            _logger      = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("[BulkJob] Processor started");

            await foreach (var job in _queue.ReadAllAsync(ct))
            {
                await ProcessAsync(job, ct);
            }
        }

        private async Task ProcessAsync(BulkJobRequest job, CancellationToken ct)
        {
            _registry.Update(job.JobId, s =>
            {
                s.Status    = BulkJobStatus.Running;
                s.StartedAt = DateTime.UtcNow;
            });

            _logger.LogInformation(
                "[BulkJob] Start JobId={JobId} Type={Type} Products={Count}",
                job.JobId, job.Type, job.ProductIds.Count);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                _notifier ??= scope.ServiceProvider.GetService<IInventoryNotifier>();

                // Lấy DbContext scoped — dùng GetRequiredService với string name để tránh reference loop
                var db = scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();

                int success = 0, fail = 0;

                // Chia productIds thành batches
                var batches = job.ProductIds
                    .Select((id, idx) => (id, idx))
                    .GroupBy(x => x.idx / BatchSize)
                    .Select(g => g.Select(x => x.id).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        var products = await db.Set<Product>()
                            .Where(p => batch.Contains(p.Id))
                            .ToListAsync(ct);

                        foreach (var p in products)
                        {
                            ApplyJobToProduct(p, job);
                            success++;
                        }

                        await db.SaveChangesAsync(ct);

                        // Detach để tránh memory leak trên long-running loop
                        foreach (var p in products)
                            db.Entry(p).State = EntityState.Detached;
                    }
                    catch (Exception ex)
                    {
                        fail += batch.Count;
                        _logger.LogWarning(ex, "[BulkJob] Batch failed for JobId={JobId}", job.JobId);
                    }

                    _registry.Update(job.JobId, s =>
                    {
                        s.ProcessedCount = success + fail;
                        s.SuccessCount   = success;
                        s.FailCount      = fail;
                    });

                    // Push progress update qua SignalR
                    await PushProgressAsync(job.JobId, ct);

                    // Nhường CPU một chút giữa các batch
                    await Task.Delay(10, ct);
                }

                _registry.Update(job.JobId, s =>
                {
                    s.Status        = BulkJobStatus.Completed;
                    s.CompletedAt   = DateTime.UtcNow;
                    s.SuccessCount  = success;
                    s.FailCount     = fail;
                    s.ProcessedCount = success + fail;
                });

                _logger.LogInformation(
                    "[BulkJob] Done JobId={JobId} Success={S} Fail={F}",
                    job.JobId, success, fail);

                // Push completion notification
                await PushCompletedAsync(job.JobId, success, fail, ct);
            }
            catch (Exception ex)
            {
                _registry.Update(job.JobId, s =>
                {
                    s.Status       = BulkJobStatus.Failed;
                    s.CompletedAt  = DateTime.UtcNow;
                    s.ErrorMessage = ex.Message;
                });
                _logger.LogError(ex, "[BulkJob] Failed JobId={JobId}", job.JobId);
                await PushFailedAsync(job.JobId, ex.Message, ct);
            }
        }

        private static void ApplyJobToProduct(Product p, BulkJobRequest job)
        {
            switch (job.Type)
            {
                case BulkJobType.ApplyDiscount:
                    if (job.Payload.DiscountValue.HasValue)
                        p.Discount = Math.Clamp(job.Payload.DiscountValue.Value, 0, 100);
                    break;

                case BulkJobType.ApplyPrice:
                    if (job.Payload.AbsolutePrice.HasValue)
                    {
                        p.Price = job.Payload.AbsolutePrice.Value;
                    }
                    else if (job.Payload.PriceChangePercent.HasValue)
                    {
                        var factor = 1 + job.Payload.PriceChangePercent.Value / 100m;
                        p.Price = Math.Max(0, Math.Round(p.Price * factor, 0));
                    }
                    break;

                case BulkJobType.ToggleStatus:
                    p.IsActive = job.Payload.IsActive ?? !p.IsActive;
                    break;

                case BulkJobType.Delete:
                    p.IsActive = false; // soft delete — tắt thay vì xoá hẳn
                    break;
            }
        }

        private async Task PushProgressAsync(string jobId, CancellationToken ct)
        {
            if (_notifier == null) return;
            var state = _registry.Get(jobId);
            if (state == null) return;
            try
            {
                await _notifier.BulkJobProgressAsync(jobId, state.ProgressPercent, state.ProcessedCount, state.TotalCount, ct);
            }
            catch { /* non-critical */ }
        }

        private async Task PushCompletedAsync(
            string jobId, int success, int fail, CancellationToken ct)
        {
            if (_notifier == null) return;
            var state = _registry.Get(jobId);
            try
            {
                await _notifier.BulkJobCompletedAsync(jobId, success, fail, state?.Type.ToString(), ct);
            }
            catch { /* non-critical */ }
        }

        private async Task PushFailedAsync(string jobId, string error, CancellationToken ct)
        {
            if (_notifier == null) return;
            try
            {
                await _notifier.BulkJobFailedAsync(jobId, error, ct);
            }
            catch { /* non-critical */ }
        }
    }
}
