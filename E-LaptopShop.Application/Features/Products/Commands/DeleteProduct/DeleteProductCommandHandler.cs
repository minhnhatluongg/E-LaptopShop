using System.Threading;
using System.Threading.Tasks;
using MediatR;
using E_LaptopShop.Domain.Repositories;

namespace E_LaptopShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, int>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<int> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.DeleteAsync(request.Id, cancellationToken);
        return request.Id; // Return the deleted product's ID
    }
} 