using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// Ví tiền của user. Quan hệ 1-1 với User (UserId là PK + FK).
    /// Balance là số dư hiện tại — luôn = SUM(WalletTransaction.Amount).
    /// Không cho trừ trực tiếp; phải qua WalletTransaction.
    /// </summary>
    public partial class UserWallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        /// <summary>Số dư hiện tại (VND, decimal(18,2)).</summary>
        public decimal Balance { get; set; } = 0m;

        /// <summary>Tổng tiền đã nạp suốt vòng đời ví (chỉ tăng).</summary>
        public decimal LifetimeTopUp { get; set; } = 0m;

        /// <summary>Tổng tiền đã chi qua ví (chỉ tăng).</summary>
        public decimal LifetimeSpent { get; set; } = 0m;

        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public string? LockReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Concurrency token để chặn race condition khi nạp/trừ đồng thời.
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
