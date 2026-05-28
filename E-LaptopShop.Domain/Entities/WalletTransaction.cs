using System;
using E_LaptopShop.Domain.Enums;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// Ledger entry — mọi thay đổi số dư ví đều tạo ra 1 row ở đây.
    /// Amount dương cho TopUp/Refund/Reward; âm cho Payment/Withdraw.
    /// BalanceBefore + Amount = BalanceAfter (audit-friendly).
    /// </summary>
    public partial class WalletTransaction
    {
        public long Id { get; set; }
        public int WalletId { get; set; }
        public int UserId { get; set; } // denormalized for fast user lookup

        public WalletTransactionType Type { get; set; }

        /// <summary>Số tiền thay đổi. + cho cộng, - cho trừ.</summary>
        public decimal Amount { get; set; }

        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        /// <summary>Loại tham chiếu nguồn: "Order", "GiftCard", "Manual", "Refund", ...</summary>
        public string? ReferenceType { get; set; }

        /// <summary>ID của entity nguồn (vd OrderId, GiftCardId).</summary>
        public string? ReferenceId { get; set; }

        public string? Note { get; set; }
        public string? CreatedBy { get; set; } // userId của người thực hiện (có thể là admin)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual UserWallet Wallet { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
