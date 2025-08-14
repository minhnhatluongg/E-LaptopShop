namespace E_LaptopShop.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,        // Chờ xử lý
        Confirmed = 2,      // Đã xác nhận
        Processing = 3,     // Đang xử lý
        Shipped = 4,        // Đã giao vận
        Delivered = 5,      // Đã giao hàng
        Completed = 6,      // Hoàn thành
        Cancelled = 7,      // Đã hủy
        Returned = 8,       // Đã trả lại
        Refunded = 9        // Đã hoàn tiền
    }
    
    public enum PaymentStatus
    {
        Pending = 1,        // Chờ thanh toán
        Paid = 2,           // Đã thanh toán
        Failed = 3,         // Thanh toán thất bại
        Refunded = 4        // Đã hoàn tiền
    }
    
    public enum OrderItemStatus
    {
        Pending = 1,        // Chờ xử lý
        Confirmed = 2,      // Đã xác nhận
        OutOfStock = 3,     // Hết hàng
        Shipped = 4,        // Đã giao
        Delivered = 5,      // Đã nhận
        Returned = 6        // Đã trả lại
    }
}
