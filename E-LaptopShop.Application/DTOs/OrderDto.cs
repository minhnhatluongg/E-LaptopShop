using E_LaptopShop.Domain.Enums;
using System;
using System.Collections.Generic;

namespace E_LaptopShop.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? DiscountCode { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ShippingMethod { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Shipping Address
        public OrderShippingAddressDto? ShippingAddress { get; set; }
        
        // Order Items
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        
        // Calculated fields
        public int TotalItems => OrderItems.Sum(x => x.Quantity);
        public bool CanCancel => Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;
        public bool CanReturn => Status == OrderStatus.Delivered;
    }
    
    public class OrderShippingAddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
    }
    
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public bool IsPaid { get; set; }
        public bool CanCancel { get; set; }
    }
}
