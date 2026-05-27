using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// POCO entity — no EF attributes. See
    /// <c>Infra/Data/Configurations/OrderConfiguration.cs</c>.
    /// </summary>
    public partial class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public int? UserId { get; set; }
        public int? ShippingAddressId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public decimal SubTotal { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public string? DiscountCode { get; set; }
        public decimal TaxAmount { get; set; } = 0;
        public decimal ShippingFee { get; set; } = 0;
        public decimal TotalAmount { get; set; }

        public string? ShippingMethod { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual UserAddress? ShippingAddress { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
        public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
        public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
        public virtual ICollection<RefundTransaction> RefundTransactions { get; set; } = new List<RefundTransaction>();
    }
}
