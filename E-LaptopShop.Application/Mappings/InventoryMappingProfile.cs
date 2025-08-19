using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.Inventory.Commands.CreateInventory;
using E_LaptopShop.Application.Features.Inventory.Commands.UpdateInventory;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using System;

namespace E_LaptopShop.Application.Mappings
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetInventoryStatus(src)))
                .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.CurrentStock * src.AverageCost))
                .ForMember(dest => dest.NeedReorder, opt => opt.MapFrom(src => src.CurrentStock <= src.ReorderPoint));

            // Command to Entity mappings
            CreateMap<CreateInventoryCommand, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastPurchasePrice, opt => opt.MapFrom(src => src.AverageCost))
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.HistoryRecords, opt => opt.Ignore());

            CreateMap<UpdateInventoryCommand, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentStock, opt => opt.Ignore())
                .ForMember(dest => dest.LastPurchasePrice, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.HistoryRecords, opt => opt.Ignore());

            // DTO to Entity mappings
            CreateMap<CreateInventoryDto, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastPurchasePrice, opt => opt.MapFrom(src => src.AverageCost))
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.HistoryRecords, opt => opt.Ignore());

            CreateMap<UpdateInventoryDto, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentStock, opt => opt.Ignore())
                .ForMember(dest => dest.LastPurchasePrice, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.HistoryRecords, opt => opt.Ignore());
        }

        private string GetInventoryStatus(Inventory inventory)
        {
            if (inventory.CurrentStock == 0)
                return InventoryStatus.OutOfStock.ToString();
            
            if (inventory.CurrentStock <= inventory.MinimumStock)
                return InventoryStatus.LowStock.ToString();
            
            if (inventory.CurrentStock <= inventory.ReorderPoint)
                return InventoryStatus.Reordering.ToString();
            
            return InventoryStatus.InStock.ToString();
        }
    }
}
