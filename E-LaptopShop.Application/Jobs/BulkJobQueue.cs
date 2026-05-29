using System.Collections.Concurrent;
using System.Threading.Channels;

namespace E_LaptopShop.Application.Jobs
{
    /// <summary>
    /// Thread-safe in-memory queue dùng System.Threading.Channels.
    /// Không cần DB, không cần Hangfire — đủ dùng cho production nhỏ/vừa.
    /// Restart server → jobs queued nhưng chưa chạy sẽ mất (acceptable tradeoff).
    /// </summary>
    public interface IBulkJobQueue
    {
        ValueTask EnqueueAsync(BulkJobRequest job, CancellationToken ct = default);
        IAsyncEnumerable<BulkJobRequest> ReadAllAsync(CancellationToken ct = default);
    }

    public class BulkJobQueue : IBulkJobQueue
    {
        // Bounded: tối đa 200 jobs chờ. Nếu đầy → caller await cho đến khi có chỗ.
        private readonly Channel<BulkJobRequest> _channel =
            Channel.CreateBounded<BulkJobRequest>(new BoundedChannelOptions(200)
            {
                FullMode        = BoundedChannelFullMode.Wait,
                SingleWriter    = false,
                SingleReader    = true,
            });

        public ValueTask EnqueueAsync(BulkJobRequest job, CancellationToken ct = default)
            => _channel.Writer.WriteAsync(job, ct);

        public IAsyncEnumerable<BulkJobRequest> ReadAllAsync(CancellationToken ct = default)
            => _channel.Reader.ReadAllAsync(ct);
    }

    /// <summary>
    /// In-memory job registry — tra trạng thái job theo JobId.
    /// Giữ 1000 jobs gần nhất (FIFO eviction).
    /// </summary>
    public class BulkJobRegistry
    {
        private readonly ConcurrentDictionary<string, BulkJobState> _jobs = new();
        private readonly Queue<string> _order = new();
        private const int MaxJobs = 1000;
        private readonly object _lock = new();

        public void Register(BulkJobState state)
        {
            lock (_lock)
            {
                _jobs[state.JobId] = state;
                _order.Enqueue(state.JobId);
                while (_order.Count > MaxJobs)
                {
                    if (_order.TryDequeue(out var old)) _jobs.TryRemove(old, out _);
                }
            }
        }

        public BulkJobState? Get(string jobId)
            => _jobs.TryGetValue(jobId, out var s) ? s : null;

        public IEnumerable<BulkJobState> GetByUser(int userId)
            => _jobs.Values.Where(j => j.CreatedByUserId == userId)
                           .OrderByDescending(j => j.CreatedAt)
                           .Take(20);

        public void Update(string jobId, Action<BulkJobState> mutate)
        {
            if (_jobs.TryGetValue(jobId, out var state))
                mutate(state);
        }
    }
}
