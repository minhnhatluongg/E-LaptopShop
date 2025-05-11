using E_LaptopShop.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetAllProductSpecifications;

public class GetAllProductSpecificationsQuery : IRequest<IEnumerable<ProductSpecificationDto>>
{
    public int? ProductId { get; set; }
} 