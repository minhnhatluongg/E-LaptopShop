using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.UpdateProductImage
{
    public class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand, ProductImageDto>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;

        public UpdateProductImageCommandHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }

        public async Task<ProductImageDto> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = _mapper.Map<Domain.Entities.ProductImage>(request);
            var updatedProductImage = await _productImageRepository.UpdateImageAsync(productImage, cancellationToken);
            var productImageDto = _mapper.Map<ProductImageDto>(updatedProductImage);
            return productImageDto;
        }
    }
}
