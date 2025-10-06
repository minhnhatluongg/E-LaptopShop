using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImageWithSysFile
{
    public class CreateProductImageWithSysFileCommandHandler : IRequestHandler<CreateProductImageWithSysFileCommand, ProductImageDto>
    {
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductImageWithSysFileCommandHandler> _logger;

        public CreateProductImageWithSysFileCommandHandler(
            IProductImageService productImageService,
            IMapper mapper,
            ILogger<CreateProductImageWithSysFileCommandHandler> logger
            )
        {
            _productImageService = productImageService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ProductImageDto> Handle(CreateProductImageWithSysFileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProductImageWithSysFileCommand for ProductId: {ProductId}, SysFileId: {SysFileId}",
                request.ProductId, request.SysFileId);

            return await _productImageService.CreateWithSysFileAsync(
            productId: request.ProductId,
            sysFileId: request.SysFileId,
            isMain: request.IsMain,
            altText: request.AltText,
            title: request.Title,
            createdBy: request.CreatedBy,
            cancellationToken
            );
        }
    }
}
