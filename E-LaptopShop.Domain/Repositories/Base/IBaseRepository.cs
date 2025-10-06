using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace E_LaptopShop.Domain.Repositories.Base
{
    /// <summary>
    /// Base repository interface providing comprehensive CRUD operations and query capabilities
    /// Designed to work seamlessly with BaseService pattern for consistent data access
    /// </summary>
    /// <typeparam name="TEntity">The entity type this repository manages</typeparam>
    /// <typeparam name="TKey">The primary key type (typically int)</typeparam>
    public interface IBaseRepository<TEntity, TKey> where TEntity : class
    {
        #region Basic CRUD Operations

        /// <summary>
        /// Gets an entity by its primary key
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity if found, null otherwise</returns>
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all entities without filtering
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Added entity with populated key</returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by its primary key
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists by its primary key
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
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

        /// <summary>
        /// Adds multiple entities in a single operation
        /// </summary>
        /// <param name="entities">Entities to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Added entities</returns>
        Task<IEnumerable<TEntity>> AddRangeAsync(
            IEnumerable<TEntity> entities, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates multiple entities in a single operation
        /// </summary>
        /// <param name="entities">Entities to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated entities</returns>
        Task<IEnumerable<TEntity>> UpdateRangeAsync(
            IEnumerable<TEntity> entities, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple entities by their primary keys
        /// </summary>
        /// <param name="ids">Primary key values</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of entities deleted</returns>
        Task<int> DeleteRangeAsync(
            IEnumerable<TKey> ids, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes entities based on a predicate
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of entities deleted</returns>
        Task<int> DeleteWhereAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default);

        #endregion

        #region Transaction Support

        /// <summary>
        /// Begins a database transaction
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Transaction object</returns>
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all pending changes to the database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of entities affected</returns>
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

    /// <summary>
    /// Base repository interface with int primary key (most common case)
    /// </summary>
    /// <typeparam name="TEntity">The entity type this repository manages</typeparam>
    public interface IBaseRepository<TEntity> : IBaseRepository<TEntity, int> where TEntity : class
    {
        // Inherits all methods from IBaseRepository<TEntity, int>
        // This is a convenience interface for entities with int primary keys
    }

    /// <summary>
    /// Transaction interface for database operations
    /// Wrapper around EF Core transactions for abstraction
    /// </summary>
    public interface IDbTransaction : IDisposable
    {
        /// <summary>
        /// Commits the transaction
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }

}
