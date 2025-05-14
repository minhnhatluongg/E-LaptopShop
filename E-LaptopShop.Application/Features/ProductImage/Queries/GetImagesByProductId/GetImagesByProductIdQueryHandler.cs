using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetImagesByProductId
{
    public class GetImagesByProductIdQueryHandler : IRequestHandler<GetImagesByProductIdQuery, IEnumerable<ProductImageDto>>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;
        public GetImagesByProductIdQueryHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductImageDto>> Handle(GetImagesByProductIdQuery request, CancellationToken cancellationToken)
        {
            var productImages = await _productImageRepository.GetImagesByProductIdAsync(request.ProductId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductImageDto>>(productImages);
        }
    }
}
