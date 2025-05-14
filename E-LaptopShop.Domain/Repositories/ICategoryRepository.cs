using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken);
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Category>> GetFilteredAsync(
        int? id = null,
        string? name = null,
        string? description = null,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<Category> Items, int totalCount)> GetAllFilterAndPagination(
        int pageNumber,
        int pageSize,
        int? id = null,
        string? name = null,
        string? description = null,
        CancellationToken cancellationToken = default
    );

} 