using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetFilteredAsync(
        int? id = null,
        string? name = null,
        string? description = null,
        CancellationToken cancellationToken = default
    );
    IQueryable<Category> GetFilteredQueryable(
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