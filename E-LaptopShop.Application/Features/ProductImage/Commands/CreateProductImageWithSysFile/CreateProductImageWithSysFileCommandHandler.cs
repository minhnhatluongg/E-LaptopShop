using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImageWithSysFile
{
    public class CreateProductImageWithSysFileCommandHandler : IRequestHandler<CreateProductImageWithSysFileCommand, ProductImageDto>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly ISysFileRepository _sysFileRepository;
        private readonly IMapper _mapper;

        public CreateProductImageWithSysFileCommandHandler(
            IProductImageRepository productImageRepository,
            ISysFileRepository sysFileRepository,
            IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _sysFileRepository = sysFileRepository;
            _mapper = mapper;
        }

        public async Task<ProductImageDto> Handle(CreateProductImageWithSysFileCommand request, CancellationToken cancellationToken)
        {
            var sysFile = await _sysFileRepository.GetByIdAsync(request.SysFileId, cancellationToken);

            var productImage = new Domain.Entities.ProductImage
            {
                ProductId = request.ProductId,
                SysFileId = request.SysFileId,
                ImageUrl = sysFile.FileUrl, 
                IsMain = request.IsMain,
                FileType = sysFile.FileType,
                FileSize = sysFile.FileSize,
                DisplayOrder = 0,
                AltText = request.AltText,
                Title = request.Title,
                CreatedAt = DateTime.Now,
                UploadedAt = DateTime.Now,
                IsActive = true,
                CreatedBy = request.CreatedBy
            };

            if (request.IsMain)
            {
                var createdProductImage = await _productImageRepository.AddImageAsync(productImage, cancellationToken);
                await _productImageRepository.SetMainImageAsync(createdProductImage.Id, cancellationToken);
                return _mapper.Map<ProductImageDto>(createdProductImage);
            }
            else
            {
                var createdProductImage = await _productImageRepository.AddImageAsync(productImage, cancellationToken);
                return _mapper.Map<ProductImageDto>(createdProductImage);
            }
        }
    }
}
