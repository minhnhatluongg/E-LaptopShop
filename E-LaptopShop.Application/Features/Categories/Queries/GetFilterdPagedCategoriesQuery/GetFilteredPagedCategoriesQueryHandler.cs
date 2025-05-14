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

        public Task<PagedResult<CategoryDto>> Handle(GetFilteredPagedCategoriesQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
