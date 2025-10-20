using AutoMapper;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.Features.Inventory.Queries.GetAllInventory;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoriEntity = E_LaptopShop.Domain.Entities.Category;

namespace E_LaptopShop.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PagedResult<CategoryDto>>
    {
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public GetAllCategoriesQueryHandler(IMapper mapper, ILogger<GetAllCategoriesQueryHandler> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public Task<PagedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.QueryParams;
            queryParams.ValidateAndNormalize();
            queryParams.ValidateBusinessRules();
            return _categoryService.GetAllAsync(queryParams, cancellationToken);
        }
    }
}
