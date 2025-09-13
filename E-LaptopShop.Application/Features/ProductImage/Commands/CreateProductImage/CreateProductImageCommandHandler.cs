using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImage
{
    class CreateProductImageCommandHandler : IRequestHandler<CreateProductImageCommand, ProductImageDto>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;

        public CreateProductImageCommandHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }

        public async Task<ProductImageDto> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = _mapper.Map<Domain.Entities.ProductImage>(request);
            
            productImage.CreatedAt = DateTime.Now;
            productImage.UploadedAt = DateTime.Now;
            productImage.IsActive = true;
            
            var createdProductImage = await _productImageRepository.AddImageAsync(productImage, cancellationToken);
            return _mapper.Map<ProductImageDto>(createdProductImage);
        }
    }
}
