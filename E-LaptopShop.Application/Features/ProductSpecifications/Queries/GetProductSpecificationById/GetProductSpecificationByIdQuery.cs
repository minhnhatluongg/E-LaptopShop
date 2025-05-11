using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetProductSpecificationById;

public class GetProductSpecificationByIdQuery : IRequest<ProductSpecificationDto>
{
    public int Id { get; set; }
} 