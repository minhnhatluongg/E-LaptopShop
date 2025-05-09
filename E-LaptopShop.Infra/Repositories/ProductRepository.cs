using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;

namespace E_LaptopShop.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving product with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving all products", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetFilteredAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStock.HasValue)
            query = inStock.Value
                ? query.Where(p => p.InStock > 0)
                : query.Where(p => p.InStock <= 0);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error adding product", ex);
        }
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = await _context.Products.FindAsync(new object[] { product.Id }, cancellationToken);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {product.Id} not found");

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync(cancellationToken);
            return existingProduct;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error updating product with ID {product.Id}", ex);
        }
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return id;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error deleting product with ID {id}", ex);
        }
    }
} 