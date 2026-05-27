using AutoMapper;
using E_LaptopShop.Application.DTOs.QueryParams___forGetAll;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class CategoriesService : BaseService<Category, CategoryDto, CategoryCreateRequestDto, CategoryUpdateRequestDto, CategoriesParams>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISlugGenerator _slugGenerator;
        public CategoriesService(
            IMapper mapper,
            ILogger<CategoriesService> logger,
            ISlugGenerator slugGenerator,
            ICategoryRepository categoryRepository
            ) : base(mapper, logger)
        {
            _slugGenerator = slugGenerator;
            _categoryRepository = categoryRepository;
        }

        protected override IQueryable<Category> ApplyBusinessFilters(IQueryable<Category> q, CategoriesParams p)
        {
            if (p.Id is > 0) q = q.Where(x => x.Id == p.Id);
            if (p.ParentId is > 0) q = q.Where(x => x.ParentId == p.ParentId);

            if (!string.IsNullOrWhiteSpace(p.Name))
            {
                var s = p.Name.Trim();
                q = q.Where(x => EF.Functions.Like(x.Name, $"%{s}%"));
            }

            if (!string.IsNullOrWhiteSpace(p.Slug))
            {
                var s = p.Slug.Trim();
                q = q.Where(x => x.Slug == s);
            }

            if (!string.IsNullOrWhiteSpace(p.Description))
            {
                var s = p.Description.Trim();
                q = q.Where(x => x.Description != null && EF.Functions.Like(x.Description, $"%{s}%"));
            }

            if (p.IsActive.HasValue) q = q.Where(x => x.IsActive == p.IsActive.Value);

            if (p.CreatedFrom.HasValue) q = q.Where(x => x.CreatedAt >= p.CreatedFrom.Value);
            if (p.CreatedTo.HasValue) q = q.Where(x => x.CreatedAt < p.CreatedTo.Value);
            if (p.UpdatedFrom.HasValue) q = q.Where(x => x.UpdatedAt >= p.UpdatedFrom.Value);
            if (p.UpdatedTo.HasValue) q = q.Where(x => x.UpdatedAt < p.UpdatedTo.Value);

            return q;
        }

        protected override IQueryable<Category> ApplyDomainSearch(IQueryable<Category> q, CategoriesParams p)
        {
            if (string.IsNullOrWhiteSpace(p.Search)) return q;

            var s = p.Search.Trim();
            return q.Where(x =>
                EF.Functions.Like(x.Name, $"%{s}%") ||
                (x.Slug != null && EF.Functions.Like(x.Slug, $"%{s}%")) ||
                (x.Description != null && EF.Functions.Like(x.Description, $"%{s}%"))
            );
        }

        protected override IQueryable<Category> ApplyDomainSorting(IQueryable<Category> q, CategoriesParams p)
        {
            return ApplySortingByMap(q, p.SortBy, p.IsAscending);
        }

        protected override async Task<Category> CreateEntityAsync(Category entity, CancellationToken ct)
        {
            return await _categoryRepository.AddAsync(entity, ct);
        }

        protected override async Task<bool> DeleteEntityAsync(int id, CancellationToken ct)
        {
            return await _categoryRepository.DeleteAsync(id, ct);
        }

        protected override IQueryable<Category> GetBaseQueryable(CategoriesParams queryParams)
        {
            return _categoryRepository.GetQueryable();
        }

        protected override Task<Category?> GetEntityByIdAsync(int id, CancellationToken ct)
        {
            return _categoryRepository.GetByIdAsync(id, ct);
        }

        protected override Task<Category> UpdateEntityAsync(Category entity, CancellationToken ct)
        {
            return _categoryRepository.UpdateAsync(entity, ct);
        }
    }
}
