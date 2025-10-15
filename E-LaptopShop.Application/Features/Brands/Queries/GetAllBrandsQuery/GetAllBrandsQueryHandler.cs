using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Features.Products.Queries.GetAllProducts;
using E_LaptopShop.Application.Services.Implementations;
using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Queries.GetAllBrandsQuery
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, PagedResult<BrandDto>>
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<GetAllBrandsQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllBrandsQueryHandler(
            IBrandService brandService,
            ILogger<GetAllBrandsQueryHandler> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _brandService = brandService;
        }
        public async Task<PagedResult<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.QueryParams;
            queryParams.ValidateAndNormalize();
            queryParams.ValidateBusinessRules();
            return await _brandService.GetAllAsync(queryParams, cancellationToken);
        }
    }
}
