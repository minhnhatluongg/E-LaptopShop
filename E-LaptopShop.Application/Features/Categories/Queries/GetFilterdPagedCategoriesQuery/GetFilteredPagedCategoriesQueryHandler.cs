using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetFilterdPagedCategoriesQuery
{
    public class GetFilteredPagedCategoriesQueryHandler : IRequestHandler<GetFilteredPagedCategoriesQuery, PagedResult<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetFilteredPagedCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<CategoryDto>> Handle(GetFilteredPagedCategoriesQuery request, CancellationToken cancellationToken)
        {
            var (categories, totalCount) = await _categoryRepository.GetAllFilterAndPagination(
                request.PageNumber,
                request.PageSize,
                request.Id,
                request.Name,
                request.Description,
                cancellationToken
            );
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return new PagedResult<CategoryDto>(
                categoryDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
