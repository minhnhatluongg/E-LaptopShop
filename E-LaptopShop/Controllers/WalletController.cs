using System.Security.Claims;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1/wallet")]
    public class WalletController : ApiV1ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        private int GetUserId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // ── Customer ─────────────────────────────────────────────────────────

        /// <summary>
        /// 👤 [CUSTOMER] Xem số dư ví của bản thân.
        /// Ví tự tạo nếu chưa tồn tại.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<WalletDto>>> GetMyWallet(CancellationToken ct)
        {
            try
            {
                var dto = await _walletService.GetMyWalletAsync(GetUserId(), ct);
                return Ok(ApiResponse<WalletDto>.SuccessResponse(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet");
                return BadRequest(ApiResponse<WalletDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// 👤 [CUSTOMER] Xem lịch sử giao dịch ví.
        /// </summary>
        [HttpGet("me/transactions")]
        [Authorize]
        [Tags(ApiTags.Customer)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WalletTransactionDto>>>> GetMyTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            try
            {
                var txs = await _walletService.GetMyTransactionsAsync(GetUserId(), pageNumber, pageSize, ct);
                return Ok(ApiResponse<IEnumerable<WalletTransactionDto>>.SuccessResponse(txs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions");
                return BadRequest(ApiResponse<IEnumerable<WalletTransactionDto>>.ErrorResponse(ex.Message));
            }
        }

        // ── Admin ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 👑 [ADMIN] Xem ví của bất kỳ user nào.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<WalletDto>>> GetUserWallet(int userId, CancellationToken ct)
        {
            try
            {
                var dto = await _walletService.GetWalletByUserIdAsync(userId, ct);
                return Ok(ApiResponse<WalletDto>.SuccessResponse(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet for user {UserId}", userId);
                return BadRequest(ApiResponse<WalletDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// 👑 [ADMIN] Nạp tiền vào ví user.
        /// </summary>
        [HttpPost("topup")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<WalletTransactionDto>>> TopUp(
            [FromBody] TopUpWalletDto dto, CancellationToken ct)
        {
            try
            {
                var tx = await _walletService.TopUpAsync(GetUserId(), dto, ct);
                return Ok(ApiResponse<WalletTransactionDto>.SuccessResponse(tx, "Nạp tiền thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error topping up wallet");
                return BadRequest(ApiResponse<WalletTransactionDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// 👑 [ADMIN] Điều chỉnh số dư ví (dương = cộng, âm = trừ).
        /// </summary>
        [HttpPost("adjust")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<WalletTransactionDto>>> Adjust(
            [FromBody] AdjustWalletDto dto, CancellationToken ct)
        {
            try
            {
                var tx = await _walletService.AdjustAsync(GetUserId(), dto, ct);
                return Ok(ApiResponse<WalletTransactionDto>.SuccessResponse(tx, "Điều chỉnh thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting wallet");
                return BadRequest(ApiResponse<WalletTransactionDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// 👑 [ADMIN] Khoá / mở khoá ví user.
        /// </summary>
        [HttpPut("user/{userId:int}/lock")]
        [AdminOnly]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<WalletDto>>> SetLock(
            int userId, [FromBody] LockWalletDto dto, CancellationToken ct)
        {
            try
            {
                var wallet = await _walletService.SetLockAsync(userId, dto.IsLocked, dto.Reason, ct);
                var msg = dto.IsLocked ? "Ví đã bị khoá" : "Ví đã được mở khoá";
                return Ok(ApiResponse<WalletDto>.SuccessResponse(wallet, msg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting wallet lock for user {UserId}", userId);
                return BadRequest(ApiResponse<WalletDto>.ErrorResponse(ex.Message));
            }
        }
    }
}
