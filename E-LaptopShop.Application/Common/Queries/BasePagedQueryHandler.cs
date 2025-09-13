using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using Microsoft.EntityFrameworkCore;
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
        // <summary>
        /// Phương thức này sẽ được gọi trong ProcessQueryOptimized để lấy IQueryable thay vì IEnumerable
        /// Enhanced version of ProcessQuery that uses IQueryable for better performance
        protected async Task<PagedResult<TDto>> ProcessQueryOptimized(TQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing optimized {Query}", typeof(TQuery).Name);

                // 0) Chuẩn hóa paging
                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                // 1) Lọc cứng
                var filtered = await GetFilteredQueryable(request, cancellationToken);
                filtered = filtered.AsNoTracking();

                // 2) Search (nếu có)
                if (request.SearchOptions?.HasSearch == true)
                    filtered = ApplyDatabaseSearch(filtered, request.SearchOptions);

                // 3) Đếm tổng trên tập đã lọc + search (chưa cần sort)
                var totalCount = await filtered.CountAsync(cancellationToken);

                // 4) Sort (nếu có) ngược lại dùng default
                var ordered = (request.SortOptions?.HasSorting == true)
                    ? ApplyDatabaseSorting(filtered, request.SortOptions)
                    : ApplyDefaultDatabaseSorting(filtered);

                // 5) Paging
                var paged = ordered
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                // 6) **Khuyến nghị**: ProjectTo để chỉ select cột cần thiết
                var dtos = await paged
                    .AsNoTracking()
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new PagedResult<TDto>(dtos, pageNumber, pageSize, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in optimized {Query}", typeof(TQuery).Name);
                throw;
            }
        }

        // ✨ Abstract methods - phải implement trong derived classes
        protected abstract Task<IQueryable<TEntity>> GetFilteredQueryable(TQuery request, CancellationToken cancellationToken);
        protected abstract IQueryable<TEntity> ApplyDatabaseSearch(IQueryable<TEntity> queryable, SearchOptions search);
        protected abstract IQueryable<TEntity> ApplyDatabaseSorting(IQueryable<TEntity> queryable, SortingOptions sort);

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

            return entities; 
        }

        // ✨ Virtual method cho default sorting
        protected virtual IQueryable<TEntity> ApplyDefaultDatabaseSorting(IQueryable<TEntity> queryable)
        {
            // Sử dụng reflection để tìm Id property
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty != null)
            {
                return queryable.OrderByDescending(e => EF.Property<object>(e, "Id"));
            }
            return queryable;
        }
    }
}

