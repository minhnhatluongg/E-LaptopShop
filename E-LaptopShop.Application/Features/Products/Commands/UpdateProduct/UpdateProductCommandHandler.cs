using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Application.DTOs;

namespace E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
        }

        _mapper.Map(request, existingProduct);
        var updatedProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);
        return _mapper.Map<ProductDto>(updatedProduct);
    }
} 