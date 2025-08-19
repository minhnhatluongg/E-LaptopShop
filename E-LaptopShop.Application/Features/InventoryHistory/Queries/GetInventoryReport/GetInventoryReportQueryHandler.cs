using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryReport
{
    public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, InventoryReportDto>
    {
        private readonly IInventoryRepository _inventoryRepository;

        public GetInventoryReportQueryHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InventoryReportDto> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
        {
            var allInventory = await _inventoryRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(request.Location))
            {
                allInventory = allInventory.Where(i => i.Location == request.Location);
            }

            var inventoryList = allInventory.ToList();

            var report = new InventoryReportDto
            {
                TotalProducts = inventoryList.Count,
                InStockProducts = inventoryList.Count(i => i.CurrentStock > i.MinimumStock),
                LowStockProducts = inventoryList.Count(i => i.CurrentStock > 0 && i.CurrentStock <= i.MinimumStock),
                OutOfStockProducts = inventoryList.Count(i => i.CurrentStock == 0),
                TotalInventoryValue = inventoryList.Sum(i => i.CurrentStock * i.AverageCost),
                ReportDate = DateTime.UtcNow
            };

            return report;
        }
    }
}
