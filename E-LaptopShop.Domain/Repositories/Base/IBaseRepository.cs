using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace E_LaptopShop.Domain.Repositories.Base
{
    
    public interface IBaseRepository<TEntity, TKey> where TEntity : class
    {
        #region Basic CRUD Operations
       
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
       
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

        #endregion

        #region Query Operations

        /// <summary>
        /// Gets a queryable for advanced filtering and querying
        /// Used by BaseService for complex query operations
        /// </summary>
        /// <returns>IQueryable for building complex queries</returns>
        IQueryable<TEntity> GetQueryable();

        /// <summary>
        /// Gets entities based on a predicate
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Filtered entities</returns>
        Task<IEnumerable<TEntity>> GetWhereAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single entity based on a predicate
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Single entity or null</returns>
        Task<TEntity?> GetSingleWhereAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the first entity based on a predicate
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>First entity or null</returns>
        Task<TEntity?> GetFirstWhereAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts entities based on a predicate
        /// </summary>
        /// <param name="predicate">Filter condition (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if any entity matches the predicate
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if any entity matches</returns>
        Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        #endregion

        #region Batch Operations

        Task<IEnumerable<TEntity>> AddRangeAsync(
            IEnumerable<TEntity> entities, 
            CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> UpdateRangeAsync(
            IEnumerable<TEntity> entities, 
            CancellationToken cancellationToken = default);
        Task<int> DeleteRangeAsync(
            IEnumerable<TKey> ids, 
            CancellationToken cancellationToken = default);
        Task<int> DeleteWhereAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        #endregion

        #region Transaction Support
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion

        #region Advanced Operations (Optional - can be overridden)

        /// <summary>
        /// Gets entities with included related data
        /// Override this method in specific repositories to define relationships
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity with related data</returns>
        Task<TEntity?> GetByIdWithIncludesAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities with included related data
        /// Override this method in specific repositories to define relationships
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entities with related data</returns>
        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
    public interface IBaseRepository<TEntity> : IBaseRepository<TEntity, int> where TEntity : class
    {
        // Inherits all methods from IBaseRepository<TEntity, int>
        // This is a convenience interface for entities with int primary keys
    }

    public interface IDbTransaction : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
