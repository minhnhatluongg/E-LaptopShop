using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Implementations;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.SetMainImage
{
    public class SetMainImageCommandHandler : IRequestHandler<SetMainImageCommand, ProductImageDto>
    {
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;
        private readonly ILogger<SetMainImageCommandHandler> _logger;

        public SetMainImageCommandHandler(IProductImageService productImageService, IMapper mapper, ILogger<SetMainImageCommandHandler> logger )
        {
            _productImageService = productImageService;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ProductImageDto> Handle(SetMainImageCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                Throw.IfNullOrNonPositive(request.Id, nameof(request.Id));
            _logger.LogInformation("Handling SetMainImageCommand - ImageId: {Id}", request.Id);
            var result = _productImageService.SetMainImageAsync(request.Id, cancellationToken);
            _logger.LogInformation("SetMainImageCommand handled successfully - ImageId: {Id}", request.Id);
            return result;
        }
    }
}
