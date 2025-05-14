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

        public Task<PagedResult<ProductImageDto>> Handle(GetAllFilteredAndPagitionQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    
}
