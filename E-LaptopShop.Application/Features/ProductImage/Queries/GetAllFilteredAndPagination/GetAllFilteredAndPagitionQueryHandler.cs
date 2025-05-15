using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Queries.GetAllFilteredAndPagination
{
    public class GetAllFilteredAndPagitionQueryHandler : IRequestHandler<GetAllFilteredAndPagitionQuery, PagedResult<ProductImageDto>>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllFilteredAndPagitionQueryHandler> _logger;

        public GetAllFilteredAndPagitionQueryHandler(
            IProductImageRepository productImageRepository,
            IMapper mapper,
            ILogger<GetAllFilteredAndPagitionQueryHandler> logger)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<ProductImageDto>> Handle(GetAllFilteredAndPagitionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetAllFilteredAndPagitionQuery for ProductImages");

                // Create filter parameters
                var filter = new ProductImageFilterParams
                {
                    Id = request.Id,
                    ProductId = request.ProductId,
                    ImageUrl = request.ImageUrl,
                    IsMain = request.IsMain,
                    FileType = request.FileType,
                    FileSize = request.FileSize,
                    DisplayOrder = request.DisplayOrder,
                    AltText = request.AltText,
                    Title = request.Title,
                    CreatedAt = request.CreatedAt,
                    UploadedAt = request.UploadedAt,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy
                };

                // Get filtered and paginated data
                var (items, totalCount) = await _productImageRepository.GetAllFilterAndPagination(
                    request.PageNumber,
                    request.PageSize,
                    filter,
                    request.SortBy,
                    request.IsAscending,
                    request.SearchTerm,
                    request.SearchFields,
                    cancellationToken);

                // Map to DTOs
                var dtos = _mapper.Map<IEnumerable<ProductImageDto>>(items);

                // Create paged result
                var result = new PagedResult<ProductImageDto>(
                    dtos,
                    request.PageNumber,
                    request.PageSize,
                    totalCount);

                _logger.LogInformation($"Successfully retrieved {dtos.Count()} product images");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAllFilteredAndPagitionQuery");
                throw;
            }
        }
    }
}
