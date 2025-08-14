namespace E_LaptopShop.Application.DTOs
{
    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsEmpty { get; set; }
    }
}
