using E_LaptopShop.Application.DTOs;
using MediatR;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Commands.CreateProductSpecification;

public class CreateProductSpecificationCommand : IRequest<ProductSpecificationDto>
{
    public int? ProductId { get; set; }
    public string? CPU { get; set; }
    public string? RAM { get; set; }
    public string? Storage { get; set; }
    public string? GPU { get; set; }
    public string? Screen { get; set; }
    public string? OS { get; set; }
    public string? Ports { get; set; }
    public string? Weight { get; set; }
    public string? Battery { get; set; }
} 