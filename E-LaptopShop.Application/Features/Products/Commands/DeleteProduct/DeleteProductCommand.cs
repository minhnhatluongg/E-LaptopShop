using MediatR;

namespace E_LaptopShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<int>
{
    public int Id { get; set; }
} 