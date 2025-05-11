using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Commands.CreateProductSpecification;

public class CreateProductSpecificationCommandHandler : IRequestHandler<CreateProductSpecificationCommand, ProductSpecificationDto>
{
    private readonly IProductSpecificationRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductSpecificationCommandHandler(IProductSpecificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductSpecificationDto> Handle(CreateProductSpecificationCommand request, CancellationToken cancellationToken)
    {
        var spec = _mapper.Map<ProductSpecification>(request);
        var createdSpec = await _repository.AddAsync(spec, cancellationToken);
        return _mapper.Map<ProductSpecificationDto>(createdSpec);
    }
} 