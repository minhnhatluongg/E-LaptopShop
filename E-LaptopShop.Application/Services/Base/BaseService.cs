using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace E_LaptopShop.Application.Services.Base
{
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto, TQueryParams>
        : IBaseService<TDto, TCreateDto, TUpdateDto, TQueryParams>
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
        where TQueryParams : PaginationParams
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseService(IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD

        public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                var entity = await GetEntityByIdAsync(id, ct);
                if (entity == null)
                {
                    _logger.LogWarning("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
                    return null;
                }
                return _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Creating new {EntityType}", typeof(TEntity).Name);
                await ValidateCreateDto(createDto, ct);

                var entity = _mapper.Map<TEntity>(createDto);
                await ApplyCreateBusinessRules(entity, createDto, ct);

                var created = await CreateEntityAsync(entity, ct);

                _logger.LogInformation("{EntityType} created successfully with ID: {Id}",
                    typeof(TEntity).Name, GetEntityId(created));

                return _mapper.Map<TDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }


        public virtual async Task<TDto> GetByIdOrThrowAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Getting {Entity} by Id {Id}", typeof(TEntity).Name, id);
            var entity = await GetEntityByIdAsync(id, ct);
            if (entity is null) Throw.NotFound(typeof(TEntity).Name, id);
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var existing = await GetEntityByIdAsync(id, ct)
                               ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found");

                await ValidateUpdateDto(id, updateDto, existing, ct);

                _mapper.Map(updateDto, existing);
                await ApplyUpdateBusinessRules(existing, updateDto, ct);

                var updated = await UpdateEntityAsync(existing, ct);
                _logger.LogInformation("{EntityType} updated successfully: {Id}", typeof(TEntity).Name, id);

                return _mapper.Map<TDto>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var existing = await GetEntityByIdAsync(id, ct);
                if (existing == null)
                {
                    _logger.LogWarning("{EntityType} with ID {Id} not found for deletion", typeof(TEntity).Name, id);
                    return false;
                }

                await ValidateDeleteRules(existing, ct);
                var result = await DeleteEntityAsync(id, ct);

                _logger.LogInformation("{EntityType} deleted successfully: {Id}", typeof(TEntity).Name, id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }

        #endregion

        #region Paged Query

        public virtual async Task<PagedResult<TDto>> GetAllAsync(TQueryParams p, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Executing paged query for {EntityType}", typeof(TEntity).Name);

                await ValidateQueryParams(p, ct); // có thể normalize phân trang tại đây

                // 1) Base query từ repo (không thực thi)
                var q = GetBaseQueryable(p)
                        .AsNoTracking(); // no-tracking sớm

                // 2) Business filters
                q = ApplyBusinessFilters(q, p);

                // 3) Search (tùy module)
                if (HasSearchCriteria(p))
                    q = ApplyDomainSearch(q, p);

                // 4) Đếm trước khi phân trang
                var total = await q.CountAsync(ct);

                // 5) Sorting
                q = ApplyDomainSorting(q, p);

                // 6) Paging
                var page = Math.Max(1, p.PageNumber);
                var size = Math.Max(1, p.PageSize);
                var pageQ = q.Skip((page - 1) * size).Take(size);

                // 7) Project to DTO (EF sẽ dịch sang SQL)
                var items = await pageQ
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(ct);

                _logger.LogInformation("Retrieved {Count} {EntityType} out of {Total} total",
                    items.Count, typeof(TEntity).Name, total);

                return new PagedResult<TDto>(items, page, size, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing paged query for {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }

        #endregion

        #region Abstract - Repository bindings

        protected abstract Task<TEntity?> GetEntityByIdAsync(int id, CancellationToken ct);
        protected abstract Task<TEntity> CreateEntityAsync(TEntity entity, CancellationToken ct);
        protected abstract Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken ct);
        protected abstract Task<bool> DeleteEntityAsync(int id, CancellationToken ct);

        // Lưu ý: sync, trả IQueryable thuần (không thực thi)
        protected abstract IQueryable<TEntity> GetBaseQueryable(TQueryParams queryParams);
        protected abstract IQueryable<TEntity> ApplyBusinessFilters(IQueryable<TEntity> q, TQueryParams p);
        protected abstract IQueryable<TEntity> ApplyDomainSearch(IQueryable<TEntity> q, TQueryParams p);
        protected abstract IQueryable<TEntity> ApplyDomainSorting(IQueryable<TEntity> q, TQueryParams p);

        #endregion

        #region Virtual - có thể override

        protected virtual Task ValidateQueryParams(TQueryParams p, CancellationToken ct)
        {
            // normalize phân trang nhẹ
            p.PageNumber = Math.Max(1, p.PageNumber);
            p.PageSize = Math.Clamp(p.PageSize, 1, 200);
            return Task.CompletedTask;
        }

        protected virtual bool HasSearchCriteria(TQueryParams p) => false;

        protected virtual Task ValidateCreateDto(TCreateDto dto, CancellationToken ct) => Task.CompletedTask;
        protected virtual Task ValidateUpdateDto(int id, TUpdateDto dto, TEntity existing, CancellationToken ct) => Task.CompletedTask;
        protected virtual Task ValidateDeleteRules(TEntity entity, CancellationToken ct) => Task.CompletedTask;

        protected virtual Task ApplyCreateBusinessRules(TEntity entity, TCreateDto dto, CancellationToken ct) => Task.CompletedTask;
        protected virtual Task ApplyUpdateBusinessRules(TEntity entity, TUpdateDto dto, CancellationToken ct) => Task.CompletedTask;

        // Fallback sort
        protected virtual IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> q)
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp != null) return q.OrderByDescending(e => EF.Property<object>(e, "Id"));

            var createdAt = typeof(TEntity).GetProperty("CreatedAt");
            if (createdAt != null) return q.OrderByDescending(e => EF.Property<object>(e, "CreatedAt"));

            return q;
        }

        // SortMap: module override để có sort key → expression
        protected virtual IReadOnlyDictionary<string, Expression<Func<TEntity, object>>> SortMap
            => new Dictionary<string, Expression<Func<TEntity, object>>>();

        // Helper dùng lại trong ApplyDomainSorting của module
        protected IQueryable<TEntity> ApplySortingByMap(
            IQueryable<TEntity> q,
            string? sortBy,
            bool asc)
        {
            if (!string.IsNullOrWhiteSpace(sortBy) &&
                SortMap.TryGetValue(sortBy, out var key))
                return asc ? q.OrderBy(key) : q.OrderByDescending(key);

            return ApplyDefaultSorting(q);
        }

        protected virtual object GetEntityId(TEntity entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");
            return idProperty?.GetValue(entity) ?? "Unknown";
        }

        #endregion
    }
}
