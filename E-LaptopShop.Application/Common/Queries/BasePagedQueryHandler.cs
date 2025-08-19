using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace E_LaptopShop.Application.Common.Queries
{
    /// <summary>
    /// Base handler cho tất cả paged queries với search, sort, filter logic
    /// </summary>
    public abstract class BasePagedQueryHandler<TEntity, TDto, TQuery>
        where TEntity : class
        where TDto : class
        where TQuery : BasePagedQuery<TDto>
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BasePagedQueryHandler(IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Main processing method - template pattern
        /// </summary>
        protected async Task<PagedResult<TDto>> ProcessQuery(TQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Processing {typeof(TQuery).Name} with comprehensive filtering");

                // 1. Get filtered data từ repository
                var entities = await GetFilteredEntities(request, cancellationToken);

                // 2. Apply search if provided
                if (request.Search.HasSearch)
                {
                    entities = ApplySearch(entities, request.Search);
                }

                // 3. Apply sorting
                if (request.Sort.HasSorting)
                {
                    entities = ApplySorting(entities, request.Sort);
                }
                else
                {
                    entities = ApplyDefaultSorting(entities);
                }

                // 4. Get total count before pagination
                var totalCount = entities.Count();

                // 5. Apply pagination
                var skip = (request.PageNumber - 1) * request.PageSize;
                var pagedEntities = entities.Skip(skip).Take(request.PageSize);

                // 6. Map to DTO
                var dtos = _mapper.Map<IEnumerable<TDto>>(pagedEntities);

                _logger.LogInformation($"Successfully retrieved {dtos.Count()} {typeof(TDto).Name} out of {totalCount} total");

                // 7. Return PagedResult
                return new PagedResult<TDto>(
                    dtos,
                    request.PageNumber,
                    request.PageSize,
                    totalCount
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while processing {typeof(TQuery).Name}");
                throw;
            }
        }

        // ✨ Abstract methods - phải implement trong derived classes
        protected abstract Task<IEnumerable<TEntity>> GetFilteredEntities(TQuery request, CancellationToken cancellationToken);
        protected abstract IEnumerable<TEntity> ApplySearch(IEnumerable<TEntity> entities, SearchOptions search);
        protected abstract IEnumerable<TEntity> ApplySorting(IEnumerable<TEntity> entities, SortingOptions sort);

        // ✨ Virtual methods - có thể override
        protected virtual IEnumerable<TEntity> ApplyDefaultSorting(IEnumerable<TEntity> entities)
        {
            // Default: sort by Id descending nếu có property Id
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty != null)
            {
                return entities.OrderByDescending(e => idProperty.GetValue(e));
            }

            // Nếu không có Id, sort by date properties
            var dateProperties = typeof(TEntity).GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                .Where(p => p.Name.Contains("Created") || p.Name.Contains("Updated") || p.Name.Contains("Date"))
                .FirstOrDefault();

            if (dateProperties != null)
            {
                return entities.OrderByDescending(e => dateProperties.GetValue(e));
            }

            return entities; // No default sorting
        }
    }
}

