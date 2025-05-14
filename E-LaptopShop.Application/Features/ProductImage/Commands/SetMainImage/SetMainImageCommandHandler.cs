using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.SetMainImage
{
    public class SetMainImageCommandHandler : IRequestHandler<SetMainImageCommand, ProductImageDto>
    {
        public readonly IProductImageRepository _productImageRepository;
        public readonly IMapper _mapper;

        public SetMainImageCommandHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }
        public async Task<ProductImageDto> Handle(SetMainImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = await _productImageRepository.SetMainImageAsync(request.Id, cancellationToken);
            return _mapper.Map<ProductImageDto>(productImage);
        }
    }
}
