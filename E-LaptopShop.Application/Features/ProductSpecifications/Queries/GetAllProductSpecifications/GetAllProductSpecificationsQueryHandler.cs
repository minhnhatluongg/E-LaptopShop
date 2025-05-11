using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductSpecifications.Queries.GetAllProductSpecifications;

public class GetAllProductSpecificationsQueryHandler : IRequestHandler<GetAllProductSpecificationsQuery, IEnumerable<ProductSpecificationDto>>
{
    private readonly IProductSpecificationRepository _repository;
    private readonly IMapper _mapper;

    public GetAllProductSpecificationsQueryHandler(IProductSpecificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductSpecificationDto>> Handle(GetAllProductSpecificationsQuery request, CancellationToken cancellationToken)
    {
        var specs = request.ProductId.HasValue
            ? await _repository.GetByProductIdAsync(request.ProductId.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<ProductSpecificationDto>>(specs);
    }
} 