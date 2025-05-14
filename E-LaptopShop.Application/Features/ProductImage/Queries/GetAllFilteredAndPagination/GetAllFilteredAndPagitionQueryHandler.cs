using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
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

        public GetAllFilteredAndPagitionQueryHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ProductImageDto>> Handle(GetAllFilteredAndPagitionQuery request, CancellationToken cancellationToken)
        {
            var (productImages, totalCount) = await _productImageRepository.GetAllFilterAndPagination(
                request.PageNumber,
                request.PageSize,
                request.Id,
                request.ProductId,
                request.ImageUrl,
                request.IsMain,
                request.FileType,
                request.FileSize,
                request.DisplayOrder,
                request.AltText,
                request.Title,
                request.CreatedAt,
                request.UploadedAt,
                request.IsActive,
                request.CreatedBy,
                cancellationToken
            );
            var productImageDtos = _mapper.Map<IEnumerable<ProductImageDto>>(productImages);
            return new PagedResult<ProductImageDto>(
                productImageDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
    
}
