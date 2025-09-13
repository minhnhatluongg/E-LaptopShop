using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using System;
using System.Linq;

namespace E_LaptopShop.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetStatusDisplay(src.Status)))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetStatusDisplay(src.Status)))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.OrderItems.Sum(x => x.Quantity)))
                .ForMember(dest => dest.CanCancel, opt => opt.MapFrom(src => CanCancelOrder(src.Status)));

            CreateMap<UserAddress, OrderShippingAddressDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AddressLine))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => (string?)null));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderItemStatus>(src.Status)))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetItemStatusDisplay(src.Status)))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.SKU))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => GetProductImageUrl(src.Product)));
        }

        private string GetStatusDisplay(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                return orderStatus switch
                {
                    OrderStatus.Pending => "Chờ xử lý",
                    OrderStatus.Confirmed => "Đã xác nhận",
                    OrderStatus.Processing => "Đang xử lý",
                    OrderStatus.Shipped => "Đã giao vận",
                    OrderStatus.Delivered => "Đã giao hàng",
                    OrderStatus.Completed => "Hoàn thành",
                    OrderStatus.Cancelled => "Đã hủy",
                    OrderStatus.Returned => "Đã trả lại",
                    OrderStatus.Refunded => "Đã hoàn tiền",
                    _ => status
                };
            }
            return status;
        }

        private string GetItemStatusDisplay(string status)
        {
            if (Enum.TryParse<OrderItemStatus>(status, out var itemStatus))
            {
                return itemStatus switch
                {
                    OrderItemStatus.Pending => "Chờ xử lý",
                    OrderItemStatus.Confirmed => "Đã xác nhận",
                    OrderItemStatus.OutOfStock => "Hết hàng",
                    OrderItemStatus.Shipped => "Đã giao",
                    OrderItemStatus.Delivered => "Đã nhận",
                    OrderItemStatus.Returned => "Đã trả lại",
                    _ => status
                };
            }
            return status;
        }

        private bool CanCancelOrder(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                return orderStatus == OrderStatus.Pending || orderStatus == OrderStatus.Confirmed;
            }
            return false;
        }

        private string? GetProductImageUrl(Product? product)
        {
            if (product?.ProductImages != null && product.ProductImages.Any())
            {
                var mainImage = product.ProductImages.FirstOrDefault(pi => pi.IsMain == true);
                if (mainImage?.SysFile?.FileUrl != null)
                {
                    return mainImage.SysFile.FileUrl;
                }

                var firstImage = product.ProductImages.FirstOrDefault();
                if (firstImage?.SysFile?.FileUrl != null)
                {
                    return firstImage.SysFile.FileUrl;
                }
            }
            return null;
        }
    }
}
