using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Commands.UpdateProductSpecification;

public class UpdateProductSpecificationCommandHandler : IRequestHandler<UpdateProductSpecificationCommand, ProductSpecificationDto>
{
    private readonly IProductSpecificationRepository _repository;
    private readonly IMapper _mapper;

    public UpdateProductSpecificationCommandHandler(IProductSpecificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductSpecificationDto> Handle(UpdateProductSpecificationCommand request, CancellationToken cancellationToken)
    {
        var spec = _mapper.Map<ProductSpecification>(request);
        var updatedSpec = await _repository.UpdateAsync(spec, cancellationToken);
        return _mapper.Map<ProductSpecificationDto>(updatedSpec);
    }
} 