using AutoMapper;
using E_LaptopShop.Application.DTOs;
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
    public class BrandService : BaseService<Brand, BrandDto, CreateBrandRequestDto, UpdateBrandRequestDto, BrandQueryParams>, IBrandService
    {
        private readonly IBrandRepositoy _brandRepository;
        private readonly ISlugGenerator _slugGenerator;
        public BrandService(
            IBrandRepositoy brandRepository,
            IMapper mapper,
            ISlugGenerator slugGenerator,
            ILogger<BrandService> logger) : base(mapper, logger)
        {
            _brandRepository = brandRepository;
            _slugGenerator = slugGenerator;
        }
        public async Task<IEnumerable<BrandDto>> GetActiveBrandsAsync(CancellationToken cancellationToken = default)
        {
            var activeBrands = await _brandRepository.GetAllActiveAsync(cancellationToken);
            return _mapper.Map<IEnumerable<BrandDto>>(activeBrands);
        }

        protected override IQueryable<Brand> ApplyBusinessFilters(IQueryable<Brand> q, BrandQueryParams p)
        {
            if(p.IsActive.HasValue)
            {
                q = q.Where(b => b.IsActive == p.IsActive.Value);
            }
            return q;
        }

        protected override IQueryable<Brand> ApplyDomainSearch(IQueryable<Brand> q, BrandQueryParams p)
        {
            if(!string.IsNullOrWhiteSpace(p.Name))
            {
                var searchTerm = p.Name.Trim(); 
                q = q.Where(x =>
                    EF.Functions.Like(x.Name, $"%{searchTerm}%") ||
                    EF.Functions.Like(x.Description ?? "", $"%{searchTerm}%"));
            }
            return q;
        }

        protected override IQueryable<Brand> ApplyDomainSorting(IQueryable<Brand> q, BrandQueryParams p)
        {
            return ApplySortingByMap(q, p.SortBy, p.IsAscending);
        }

        protected override async Task<Brand> CreateEntityAsync(Brand entity, CancellationToken ct)
        {
            return await _brandRepository.AddAsync(entity, ct);
        }

        protected override async Task<bool> DeleteEntityAsync(int id, CancellationToken ct)
        {
            return await _brandRepository.DeleteAsync(id, ct);
        }

        protected override IQueryable<Brand> GetBaseQueryable(BrandQueryParams queryParams)
        {
            return _brandRepository.GetQueryable();
        }

        protected override async Task<Brand?> GetEntityByIdAsync(int id, CancellationToken ct)
        {
            return await _brandRepository.GetByIdAsync(id, ct);
        }

        protected override async Task<Brand> UpdateEntityAsync(Brand entity, CancellationToken ct)
        {
            return await _brandRepository.UpdateAsync(entity, ct);
        }

        protected override async Task ApplyCreateBusinessRules(Brand entity, CreateBrandRequestDto dto, CancellationToken ct)
        {
            await base.ApplyCreateBusinessRules(entity, dto, ct);

            if (string.IsNullOrEmpty(entity.Slug))
            {
                var slug = await _slugGenerator.GenerateSlugAsync(
                    entity.Name,
                    entitySet: "Brand",
                    excludeId: (int?)null, 
                    ct);

                entity.Slug = slug;
            }
        }
    }
}
