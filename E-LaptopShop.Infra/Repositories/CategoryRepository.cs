using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Domain.Repositories.Base;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger) : base(context, logger)
    {
    }

    #region Overrides from BaseRepository
    public override async Task<Category?> GetByIdWithIncludesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .Include(c => c.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Category>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .Include(c => c.Products)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    #endregion

    public async Task<(IEnumerable<Category> Items, int totalCount)> GetAllFilterAndPagination(int pageNumber, int pageSize, int? id = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Set<Category>().AsQueryable();

            if (id.HasValue)
                query = query.Where(c => c.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var searchDesc = description.Trim().ToLower();
                query = query.Where(c => c.Description != null &&
                                         c.Description.ToLower().Contains(searchDesc));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(c => c.Products)
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllFilterAndPagination");
            throw new InvalidOperationException("Error in GetAllFilterAndPagination", ex);
        }
    }

    public async Task<IEnumerable<Category>> GetFilteredAsync(int? id = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Set<Category>().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            if (!string.IsNullOrEmpty(name))
            {
                var searchName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchName));
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                var searchDes = description.Trim().ToLower();
                query = query.Where(c => c.Description != null && c.Description.ToLower().Contains(searchDes));
            }
            return await query
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering categories");
            throw new InvalidOperationException("Error filtering categories", ex);
        }
    }

    public IQueryable<Category> GetFilteredQueryable(int? id = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Category>()
                .Include(c => c.Products)
                .AsNoTracking()
                .AsQueryable();

        if (id.HasValue)
            query = query.Where(c => c.Id == id.Value);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.Contains(name.Trim()));

        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(c => c.Description != null &&
                                     c.Description.Contains(description.Trim()));

        return query;
    }
}