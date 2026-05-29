namespace E_LaptopShop.Application.Jobs
{
    public enum BulkJobType
    {
        ApplyDiscount,   // Set/change discount %
        ApplyPrice,      // +/-% hoặc absolute price mới
        ToggleStatus,    // Bật/tắt IsActive
        Delete,          // Xoá hàng loạt
    }

    public enum BulkJobStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
    }

    /// <summary>Request đưa vào Channel queue.</summary>
    public class BulkJobRequest
    {
        public string JobId { get; init; } = Guid.NewGuid().ToString("N")[..8];
        public BulkJobType Type { get; init; }
        public List<int> ProductIds { get; init; } = new();
        public BulkJobPayload Payload { get; init; } = new();
        public int CreatedByUserId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }

    /// <summary>Tham số cho từng loại job.</summary>
    public class BulkJobPayload
    {
        /// <summary>Discount tuyệt đối để set (ví dụ: 10 = 10%).</summary>
        public decimal? DiscountValue { get; init; }

        /// <summary>+/- % so với giá hiện tại (ví dụ: -5 = giảm 5%, +10 = tăng 10%).</summary>
        public decimal? PriceChangePercent { get; init; }

        /// <summary>Giá tuyệt đối mới.</summary>
        public decimal? AbsolutePrice { get; init; }

        /// <summary>Active status mới cho ToggleStatus job.</summary>
        public bool? IsActive { get; init; }
    }

    /// <summary>Trạng thái job — lưu in-memory (ConcurrentDictionary).</summary>
    public class BulkJobState
    {
        public string JobId { get; set; } = null!;
        public BulkJobType Type { get; set; }
        public BulkJobStatus Status { get; set; } = BulkJobStatus.Queued;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public string? ErrorMessage { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public double ProgressPercent =>
            TotalCount == 0 ? 0 : Math.Round((double)ProcessedCount / TotalCount * 100, 1);

        public string StatusLabel => Status switch
        {
            BulkJobStatus.Queued    => "Đang chờ xử lý",
            BulkJobStatus.Running   => $"Đang xử lý... {ProgressPercent}%",
            BulkJobStatus.Completed => $"Hoàn thành — {SuccessCount}/{TotalCount} thành công",
            BulkJobStatus.Failed    => $"Thất bại: {ErrorMessage}",
            _                       => "Unknown",
        };
    }
}
