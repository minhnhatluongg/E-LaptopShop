namespace E_LaptopShop.Domain.Enums
{
    /// <summary>
    /// Loại giao dịch ví. Mọi thay đổi số dư đều phải có 1 row WalletTransaction
    /// với type tương ứng — ledger pattern.
    /// </summary>
    public enum WalletTransactionType
    {
        TopUp = 1,        // Nạp tiền (admin nạp / gift card / chuyển khoản)
        Payment = 2,      // Trừ tiền do thanh toán đơn hàng
        Refund = 3,       // Hoàn tiền vào ví do hủy/return đơn
        Reward = 4,       // Cộng tiền thưởng (khuyến mãi, loyalty cashback)
        Adjustment = 5,   // Điều chỉnh thủ công bởi admin (+/-)
        Withdraw = 6,     // Rút tiền (nếu sau này hỗ trợ)
    }
}
