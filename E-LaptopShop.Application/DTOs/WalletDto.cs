using E_LaptopShop.Domain.Enums;

namespace E_LaptopShop.Application.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal LifetimeTopUp { get; set; }
        public decimal LifetimeSpent { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string? LockReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class WalletTransactionDto
    {
        public long Id { get; set; }
        public int WalletId { get; set; }
        public WalletTransactionType Type { get; set; }
        public string TypeLabel => Type switch
        {
            WalletTransactionType.TopUp      => "Nạp tiền",
            WalletTransactionType.Payment    => "Thanh toán",
            WalletTransactionType.Refund     => "Hoàn tiền",
            WalletTransactionType.Reward     => "Thưởng",
            WalletTransactionType.Adjustment => "Điều chỉnh",
            WalletTransactionType.Withdraw   => "Rút tiền",
            _                               => "Khác",
        };
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    // ─── Request DTOs ──────────────────────────────────────────────────────────

    /// <summary>Admin nạp/điều chỉnh ví cho user.</summary>
    public class TopUpWalletDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }

    public class AdjustWalletDto
    {
        public int UserId { get; set; }
        /// <summary>Dương = cộng, âm = trừ.</summary>
        public decimal Amount { get; set; }
        public string Note { get; set; } = null!;
    }

    public class LockWalletDto
    {
        public bool IsLocked { get; set; }
        public string? Reason { get; set; }
    }
}
