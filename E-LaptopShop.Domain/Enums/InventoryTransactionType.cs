namespace E_LaptopShop.Domain.Enums
{
    public enum InventoryTransactionType
    {
        Purchase = 1,      // Nhập hàng
        Sale = 2,          // Bán hàng 
        Return = 3,        // Trả hàng
        Adjustment = 4,    // Điều chỉnh
        Transfer = 5,      // Chuyển kho
        Damaged = 6,       // Hàng hỏng
        Expired = 7        // Hết hạn
    }
}