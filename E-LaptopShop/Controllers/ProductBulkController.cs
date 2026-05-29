using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using E_LaptopShop.Application.Jobs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1/products")]
    public class ProductBulkController : ApiV1ControllerBase
    {
        private readonly IBulkJobQueue  _queue;
        private readonly BulkJobRegistry _registry;
        private readonly ILogger<ProductBulkController> _logger;

        public ProductBulkController(
            IBulkJobQueue queue,
            BulkJobRegistry registry,
            ILogger<ProductBulkController> logger)
        {
            _queue    = queue;
            _registry = registry;
            _logger   = logger;
        }

        private int GetUserId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/v1/products/bulk
        // Returns: 202 Accepted + jobId (không block HTTP request)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// 👑 [ADMIN] Hành động hàng loạt — trả về 202 Accepted ngay lập tức.
        /// BackgroundService xử lý ngầm và push SignalR khi xong.
        /// </summary>
        [HttpPost("bulk")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        [ProducesResponseType(typeof(ApiResponse<BulkJobStartedDto>), 202)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> BulkAction(
            [FromBody] BulkActionRequestDto dto,
            CancellationToken ct)
        {
            if (dto.ProductIds == null || dto.ProductIds.Count == 0)
                return BadRequest(ApiResponse<object>.ErrorResponse("Chưa chọn sản phẩm nào"));

            if (dto.ProductIds.Count > 1000)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Tối đa 1000 sản phẩm mỗi lần — chia nhỏ và thực hiện nhiều lần"));

            // Validate payload theo job type
            var validationError = ValidatePayload(dto);
            if (validationError != null)
                return BadRequest(ApiResponse<object>.ErrorResponse(validationError));

            var jobRequest = new BulkJobRequest
            {
                Type          = dto.Type,
                ProductIds    = dto.ProductIds.Distinct().ToList(),
                Payload       = new BulkJobPayload
                {
                    DiscountValue      = dto.DiscountValue,
                    PriceChangePercent = dto.PriceChangePercent,
                    AbsolutePrice      = dto.AbsolutePrice,
                    IsActive           = dto.IsActive,
                },
                CreatedByUserId = GetUserId(),
            };

            // Đăng ký vào registry (để có thể poll status)
            _registry.Register(new BulkJobState
            {
                JobId           = jobRequest.JobId,
                Type            = jobRequest.Type,
                TotalCount      = jobRequest.ProductIds.Count,
                CreatedByUserId = jobRequest.CreatedByUserId,
                CreatedAt       = jobRequest.CreatedAt,
            });

            // Đẩy vào Channel — non-blocking (Channel bounded)
            await _queue.EnqueueAsync(jobRequest, ct);

            _logger.LogInformation(
                "[BulkAction] Queued JobId={JobId} Type={Type} Count={Count} by User={User}",
                jobRequest.JobId, jobRequest.Type, jobRequest.ProductIds.Count, GetUserId());

            // 202 Accepted — client không cần chờ, dùng SignalR hoặc poll /bulk/jobs/{jobId}
            return StatusCode(202, ApiResponse<BulkJobStartedDto>.SuccessResponse(
                new BulkJobStartedDto
                {
                    JobId      = jobRequest.JobId,
                    TotalCount = jobRequest.ProductIds.Count,
                    Message    = $"Đã nhận {jobRequest.ProductIds.Count} sản phẩm — đang xử lý nền...",
                },
                "Job queued successfully"));
        }

        /// <summary>👑 [ADMIN] Xem trạng thái job theo JobId.</summary>
        [HttpGet("bulk/jobs/{jobId}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public ActionResult<ApiResponse<BulkJobStatusDto>> GetJobStatus(string jobId)
        {
            var state = _registry.Get(jobId);
            if (state == null)
                return NotFound(ApiResponse<object>.ErrorResponse($"Job '{jobId}' không tìm thấy"));

            return Ok(ApiResponse<BulkJobStatusDto>.SuccessResponse(new BulkJobStatusDto
            {
                JobId          = state.JobId,
                Type           = state.Type.ToString(),
                Status         = state.Status.ToString(),
                StatusLabel    = state.StatusLabel,
                TotalCount     = state.TotalCount,
                ProcessedCount = state.ProcessedCount,
                SuccessCount   = state.SuccessCount,
                FailCount      = state.FailCount,
                ProgressPercent = state.ProgressPercent,
                CreatedAt      = state.CreatedAt,
                StartedAt      = state.StartedAt,
                CompletedAt    = state.CompletedAt,
                ErrorMessage   = state.ErrorMessage,
            }));
        }

        /// <summary>👑 [ADMIN] Lịch sử jobs của user hiện tại.</summary>
        [HttpGet("bulk/jobs")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public ActionResult<ApiResponse<IEnumerable<BulkJobStatusDto>>> GetMyJobs()
        {
            var jobs = _registry.GetByUser(GetUserId())
                .Select(s => new BulkJobStatusDto
                {
                    JobId          = s.JobId,
                    Type           = s.Type.ToString(),
                    Status         = s.Status.ToString(),
                    StatusLabel    = s.StatusLabel,
                    TotalCount     = s.TotalCount,
                    ProcessedCount = s.ProcessedCount,
                    SuccessCount   = s.SuccessCount,
                    FailCount      = s.FailCount,
                    ProgressPercent = s.ProgressPercent,
                    CreatedAt      = s.CreatedAt,
                    StartedAt      = s.StartedAt,
                    CompletedAt    = s.CompletedAt,
                });
            return Ok(ApiResponse<IEnumerable<BulkJobStatusDto>>.SuccessResponse(jobs));
        }

        private static string? ValidatePayload(BulkActionRequestDto dto) => dto.Type switch
        {
            BulkJobType.ApplyDiscount =>
                dto.DiscountValue == null
                    ? "Cần nhập DiscountValue (0–100)"
                    : dto.DiscountValue < 0 || dto.DiscountValue > 100
                        ? "DiscountValue phải từ 0 đến 100"
                        : null,

            BulkJobType.ApplyPrice =>
                dto.AbsolutePrice == null && dto.PriceChangePercent == null
                    ? "Cần nhập AbsolutePrice hoặc PriceChangePercent"
                    : null,

            BulkJobType.ToggleStatus =>
                dto.IsActive == null ? "Cần nhập IsActive (true/false)" : null,

            BulkJobType.Delete => null,

            _ => $"JobType '{dto.Type}' không hợp lệ",
        };
    }

    // ─── DTOs ──────────────────────────────────────────────────────────────────

    public class BulkActionRequestDto
    {
        [Required]
        [MinLength(1)]
        public List<int> ProductIds { get; set; } = new();

        [Required]
        public BulkJobType Type { get; set; }

        // ApplyDiscount
        public decimal? DiscountValue { get; set; }

        // ApplyPrice
        public decimal? PriceChangePercent { get; set; }
        public decimal? AbsolutePrice { get; set; }

        // ToggleStatus
        public bool? IsActive { get; set; }
    }

    public class BulkJobStartedDto
    {
        public string JobId { get; set; } = null!;
        public int TotalCount { get; set; }
        public string Message { get; set; } = null!;
    }

    public class BulkJobStatusDto
    {
        public string JobId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string StatusLabel { get; set; } = null!;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public double ProgressPercent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
