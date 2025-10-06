using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
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

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetImagesByProductId
{
    public class GetImagesByProductIdQueryHandler : IRequestHandler<GetImagesByProductIdQuery, IEnumerable<ProductImageDto>>
    {
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetImagesByProductIdQueryHandler> _logger;
        public GetImagesByProductIdQueryHandler(IProductImageService productImageService, IMapper mapper, ILogger<GetImagesByProductIdQueryHandler> logger)
        {
            _productImageService = productImageService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<ProductImageDto>> Handle(GetImagesByProductIdQuery request, CancellationToken cancellationToken)
        {
            Throw.IfNullOrNonPositive(request.ProductId, nameof(request.ProductId));
            var productImages = await _productImageService.GetImagesByProductIdAsync(request.ProductId, cancellationToken: cancellationToken);
            return productImages;
        }
    }
}
