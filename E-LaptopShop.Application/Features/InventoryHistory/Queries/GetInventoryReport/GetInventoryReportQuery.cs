using E_LaptopShop.Application.DTOs;
using MediatR;
using System;

namespace E_LaptopShop.Application.Features.InventoryHistory.Queries.GetInventoryReport
{
    public class GetInventoryReportQuery : IRequest<InventoryReportDto>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Location { get; set; }
    }
}
