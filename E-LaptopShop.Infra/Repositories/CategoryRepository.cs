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
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
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
} 