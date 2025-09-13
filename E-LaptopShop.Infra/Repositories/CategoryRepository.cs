using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;

namespace E_LaptopShop.Infra.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if(id <= 0 ) {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            }
            var category = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");
            return category;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving category with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Categories
                .Include(c => c.Products)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving all categories", ex);
        }
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return category;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error adding category", ex);
        }
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = await _context.Categories.FindAsync(new object[] { category.Id }, cancellationToken);
            if (existingCategory == null)
                throw new KeyNotFoundException($"Category with ID {category.Id} not found");

            _context.Entry(existingCategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync(cancellationToken);
            return existingCategory;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error updating category with ID {category.Id}", ex);
        }
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return id;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error deleting category with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Category>> GetFilteredAsync(
        int? id = null,
        string? name = null,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Categories.AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            if (!string.IsNullOrEmpty(name))
            {
                var searchName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchName));
            }
            if (!string.IsNullOrEmpty(description))
            {
                var searchDescription = description.Trim().ToLower();
                query = query.Where(c => c.Description != null && c.Description.ToLower().Contains(searchDescription));
            }

            return await query 
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error filtering categories", ex);
        }
    }

    public async Task<(IEnumerable<Category> Items, int totalCount)> GetAllFilterAndPagination(
        int pageNumber, 
        int pageSize, 
        int? id = null, 
        string? name = null, 
        string? description = null, 
        CancellationToken cancellationToken = default)
    {

        try
        {
            var query = _context.Categories.AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            if (!string.IsNullOrEmpty(name))
            {
                var searchName = name.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchName));
            }
            if (!string.IsNullOrEmpty(description))
            {
                var searchDescription = description.Trim().ToLower();
                query = query.Where(c => c.Description != null && c.Description.ToLower().Contains(searchDescription));
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
            throw new InvalidOperationException("Error filtering and paginating categories", ex);
        }

    }

    public IQueryable<Category> GetFilteredQueryable(
    int? id = null,
    string? nameExact = null,
    string? descriptionExact = null,
    bool includeProducts = false)
    {
        var q = _context.Categories
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            q = q.Where(c => c.Id == id.Value);

        if (!string.IsNullOrWhiteSpace(nameExact))
            q = q.Where(c => c.Name == nameExact.Trim());

        if (!string.IsNullOrWhiteSpace(descriptionExact))
            q = q.Where(c => c.Description == descriptionExact.Trim());

        if (includeProducts)
            q = q.Include(c => c.Products).AsSplitQuery();

        return q;
    }

    public IQueryable<Category> GetFilteredQueryable(int? id = null, string? name = null, string? description = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Categories
            .Include(c => c.Products).AsSplitQuery()
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            q = q.Where(c => c.Id == id.Value);

        if (!string.IsNullOrWhiteSpace(name))
            q = q.Where(c => c.Name == name.Trim());

        if (!string.IsNullOrWhiteSpace(description))
            q = q.Where(c => c.Description == description.Trim());
        return q;
    }
}