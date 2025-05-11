using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetProductSpecificationById;

public class GetProductSpecificationByIdQueryHandler : IRequestHandler<GetProductSpecificationByIdQuery, ProductSpecificationDto>
{
    private readonly IProductSpecificationRepository _repository;
    private readonly IMapper _mapper;

    public GetProductSpecificationByIdQueryHandler(IProductSpecificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductSpecificationDto> Handle(GetProductSpecificationByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (spec == null)
        {
            throw new KeyNotFoundException($"Product specification with ID {request.Id} not found.");
        }

        return _mapper.Map<ProductSpecificationDto>(spec);
    }
} 