using System.Threading;
using System.Threading.Tasks;
using MediatR;
using E_LaptopShop.Application.Services.Interfaces;

namespace E_LaptopShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, int>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<int> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.DeleteProductAsync(request.Id, cancellationToken);
    }
} 