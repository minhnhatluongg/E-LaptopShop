using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Enums;

namespace E_LaptopShop.Application.Services.Interfaces
{
    public interface IWalletService
    {
        // ── Customer ──────────────────────────────────────────────────────────
        Task<WalletDto> GetMyWalletAsync(int userId, CancellationToken ct = default);

        Task<IEnumerable<WalletTransactionDto>> GetMyTransactionsAsync(
            int userId, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);

        // ── Admin ─────────────────────────────────────────────────────────────
        Task<WalletDto> GetWalletByUserIdAsync(int userId, CancellationToken ct = default);

        Task<WalletTransactionDto> TopUpAsync(int adminUserId, TopUpWalletDto dto, CancellationToken ct = default);

        Task<WalletTransactionDto> AdjustAsync(int adminUserId, AdjustWalletDto dto, CancellationToken ct = default);

        Task<WalletDto> SetLockAsync(int userId, bool isLocked, string? reason, CancellationToken ct = default);

        // ── Internal (called by OrderService) ────────────────────────────────
        Task<WalletTransactionDto> PayAsync(
            int userId, decimal amount, int orderId, CancellationToken ct = default);

        Task<WalletTransactionDto> RefundAsync(
            int userId, decimal amount, int orderId, string? note = null, CancellationToken ct = default);
    }
}
