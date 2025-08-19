namespace E_LaptopShop.Domain.Enums
{
    public enum InventoryStatus
    {
        InStock = 1,       // Còn hàng
        LowStock = 2,      // Sắp hết
        OutOfStock = 3,    // Hết hàng
        Reordering = 4,    // Đang đặt hàng
        Discontinued = 5   // Ngừng kinh doanh
    }
}