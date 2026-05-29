using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IUserWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _txRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IUserWalletRepository walletRepo,
            IWalletTransactionRepository txRepo,
            IMapper mapper,
            ILogger<WalletService> logger)
        {
            _walletRepo = walletRepo;
            _txRepo = txRepo;
            _mapper = mapper;
            _logger = logger;
        }

        // ── Customer ──────────────────────────────────────────────────────────

        public async Task<WalletDto> GetMyWalletAsync(int userId, CancellationToken ct = default)
        {
            var wallet = await _walletRepo.GetOrCreateAsync(userId, ct);
            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<IEnumerable<WalletTransactionDto>> GetMyTransactionsAsync(
            int userId, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        {
            var txs = await _txRepo.GetByUserIdAsync(userId, pageNumber, pageSize, ct);
            return _mapper.Map<IEnumerable<WalletTransactionDto>>(txs);
        }

        // ── Admin ─────────────────────────────────────────────────────────────

        public async Task<WalletDto> GetWalletByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var wallet = await _walletRepo.GetOrCreateAsync(userId, ct);
            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<WalletTransactionDto> TopUpAsync(
            int adminUserId, TopUpWalletDto dto, CancellationToken ct = default)
        {
            if (dto.Amount <= 0) Throw.BadRequest("Số tiền nạp phải > 0");

            var tx = await ExecuteTransactionAsync(
                userId: dto.UserId,
                type: WalletTransactionType.TopUp,
                amount: dto.Amount,
                referenceType: "Manual",
                referenceId: adminUserId.ToString(),
                note: dto.Note ?? "Admin nạp tiền",
                createdBy: adminUserId.ToString(),
                ct: ct);

            return _mapper.Map<WalletTransactionDto>(tx);
        }

        public async Task<WalletTransactionDto> AdjustAsync(
            int adminUserId, AdjustWalletDto dto, CancellationToken ct = default)
        {
            if (dto.Amount == 0) Throw.BadRequest("Số tiền điều chỉnh phải khác 0");
            if (string.IsNullOrWhiteSpace(dto.Note)) Throw.BadRequest("Phải nhập lý do điều chỉnh");

            var type = dto.Amount > 0
                ? WalletTransactionType.Adjustment
                : WalletTransactionType.Adjustment;

            var tx = await ExecuteTransactionAsync(
                userId: dto.UserId,
                type: type,
                amount: dto.Amount,
                referenceType: "Adjustment",
                referenceId: adminUserId.ToString(),
                note: dto.Note,
                createdBy: adminUserId.ToString(),
                ct: ct);

            return _mapper.Map<WalletTransactionDto>(tx);
        }

        public async Task<WalletDto> SetLockAsync(
            int userId, bool isLocked, string? reason, CancellationToken ct = default)
        {
            var wallet = await _walletRepo.GetOrCreateAsync(userId, ct);
            wallet.IsLocked = isLocked;
            wallet.LockReason = isLocked ? reason : null;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet, ct);
            return _mapper.Map<WalletDto>(wallet);
        }

        // ── Internal ─────────────────────────────────────────────────────────

        public async Task<WalletTransactionDto> PayAsync(
            int userId, decimal amount, int orderId, CancellationToken ct = default)
        {
            if (amount <= 0) Throw.BadRequest("Số tiền thanh toán phải > 0");

            var wallet = await _walletRepo.GetOrCreateAsync(userId, ct);
            if (wallet.Balance < amount)
                Throw.BusinessRule("InsufficientBalance", "Số dư ví không đủ để thanh toán");

            var tx = await ExecuteTransactionAsync(
                userId: userId,
                type: WalletTransactionType.Payment,
                amount: -amount,
                referenceType: "Order",
                referenceId: orderId.ToString(),
                note: $"Thanh toán đơn hàng #{orderId}",
                createdBy: userId.ToString(),
                ct: ct);

            return _mapper.Map<WalletTransactionDto>(tx);
        }

        public async Task<WalletTransactionDto> RefundAsync(
            int userId, decimal amount, int orderId, string? note = null, CancellationToken ct = default)
        {
            if (amount <= 0) Throw.BadRequest("Số tiền hoàn phải > 0");

            var tx = await ExecuteTransactionAsync(
                userId: userId,
                type: WalletTransactionType.Refund,
                amount: amount,
                referenceType: "Order",
                referenceId: orderId.ToString(),
                note: note ?? $"Hoàn tiền đơn hàng #{orderId}",
                createdBy: "System",
                ct: ct);

            return _mapper.Map<WalletTransactionDto>(tx);
        }

        // ── Private core ─────────────────────────────────────────────────────

        /// <summary>
        /// Atomic ledger entry: đọc ví, tính số dư mới, ghi tx + cập nhật balance trong cùng call.
        /// RowVersion trên UserWallet chặn concurrent writes.
        /// </summary>
        private async Task<WalletTransaction> ExecuteTransactionAsync(
            int userId,
            WalletTransactionType type,
            decimal amount,
            string? referenceType,
            string? referenceId,
            string? note,
            string? createdBy,
            CancellationToken ct)
        {
            var wallet = await _walletRepo.GetOrCreateAsync(userId, ct);

            if (wallet.IsLocked)
                Throw.BusinessRule("WalletLocked", $"Ví đã bị khoá: {wallet.LockReason}");

            var balanceBefore = wallet.Balance;
            var balanceAfter = balanceBefore + amount;

            if (balanceAfter < 0)
                Throw.BusinessRule("InsufficientBalance", "Số dư không đủ");

            // Cập nhật wallet
            wallet.Balance = balanceAfter;
            wallet.UpdatedAt = DateTime.UtcNow;

            if (amount > 0 && type == WalletTransactionType.TopUp)
                wallet.LifetimeTopUp += amount;
            if (amount < 0 && type == WalletTransactionType.Payment)
                wallet.LifetimeSpent += Math.Abs(amount);

            await _walletRepo.UpdateAsync(wallet, ct);

            // Ghi transaction
            var tx = new WalletTransaction
            {
                WalletId = wallet.Id,
                UserId = userId,
                Type = type,
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Note = note,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
            };

            await _txRepo.AddAsync(tx, ct);

            _logger.LogInformation(
                "Wallet tx: User={UserId} Type={Type} Amount={Amount} Balance={Before}→{After}",
                userId, type, amount, balanceBefore, balanceAfter);

            return tx;
        }
    }
}
