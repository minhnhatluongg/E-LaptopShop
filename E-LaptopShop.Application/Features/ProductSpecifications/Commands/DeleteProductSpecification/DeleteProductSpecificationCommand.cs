using MediatR;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Commands.DeleteProductSpecification;

public class DeleteProductSpecificationCommand : IRequest<int>
{
    public int Id { get; set; }
} 