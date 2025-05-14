using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;

namespace E_LaptopShop.Infra.Repositories;

public class ProductSpecificationRepository : IProductSpecificationRepository
{
    private readonly ApplicationDbContext _context;

    public ProductSpecificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductSpecification> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if(id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            var spec = await _context.ProductSpecifications
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (spec == null)
                throw new KeyNotFoundException($"Product specification with ID {id} not found");
            return spec;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving product specification with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<ProductSpecification>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.ProductSpecifications
                .Include(p => p.Product)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving all product specifications", ex);
        }
    }

    public async Task<IEnumerable<ProductSpecification>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.ProductSpecifications
                .Include(p => p.Product)
                .Where(p => p.ProductId == productId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving product specifications for product ID {productId}", ex);
        }
    }

    public async Task<ProductSpecification> AddAsync(ProductSpecification spec, CancellationToken cancellationToken)
    {
        try
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            await _context.ProductSpecifications.AddAsync(spec, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return spec;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error adding product specification", ex);
        }
    }

    public async Task<ProductSpecification> UpdateAsync(ProductSpecification spec, CancellationToken cancellationToken)
    {
        try
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var existingSpec = await _context.ProductSpecifications.FindAsync(new object[] { spec.Id }, cancellationToken);
            if (existingSpec == null)
                throw new KeyNotFoundException($"Product specification with ID {spec.Id} not found");

            _context.Entry(existingSpec).CurrentValues.SetValues(spec);
            await _context.SaveChangesAsync(cancellationToken);
            return existingSpec;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error updating product specification with ID {spec.Id}", ex);
        }
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var spec = await _context.ProductSpecifications.FindAsync(new object[] { id }, cancellationToken);
            if (spec == null)
                throw new KeyNotFoundException($"Product specification with ID {id} not found");

            _context.ProductSpecifications.Remove(spec);
            await _context.SaveChangesAsync(cancellationToken);
            return id;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Error deleting product specification with ID {id}", ex);
        }
    }
} 