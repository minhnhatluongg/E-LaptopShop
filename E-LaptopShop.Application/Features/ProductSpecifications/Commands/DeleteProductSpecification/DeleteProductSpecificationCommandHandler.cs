using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Commands.DeleteProductSpecification;

public class DeleteProductSpecificationCommandHandler : IRequestHandler<DeleteProductSpecificationCommand, int>
{
    private readonly IProductSpecificationRepository _repository;

    public DeleteProductSpecificationCommandHandler(IProductSpecificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(DeleteProductSpecificationCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.Id, cancellationToken);
    }
} 