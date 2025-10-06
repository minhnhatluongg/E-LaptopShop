using E_LaptopShop.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DomainTransaction = E_LaptopShop.Domain.Repositories.Base.IDbTransaction;


namespace E_LaptopShop.Infra.Repositories.Base
{
    public interface ISoftDeletable 
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
   
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger _logger;

        protected bool AutoSaveChanges { get; }

        protected BaseRepository(DbContext context, ILogger logger, bool autoSaveChanges = true)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _context.Set<TEntity>();
            AutoSaveChanges = autoSaveChanges;
        }

        #region Basic CRUD Operations

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateId(id);
                _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);

                var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
                if (entity == null)
                {
                    _logger.LogDebug("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
                }

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(TEntity).Name, id);
                throw new InvalidOperationException($"Error retrieving {typeof(TEntity).Name} with ID {id}", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting all {EntityType}", typeof(TEntity).Name);

                var entities = await _dbSet
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                _logger.LogDebug("Retrieved {Count} {EntityType} entities", entities.Count, typeof(TEntity).Name);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityType}", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error retrieving all {typeof(TEntity).Name}", ex);
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateEntity(entity);
                _logger.LogDebug("Adding new {EntityType}", typeof(TEntity).Name);

                await ValidateBeforeAdd(entity, cancellationToken);
                await ApplyAddBusinessRules(entity, cancellationToken);

                var entry = await _dbSet.AddAsync(entity, cancellationToken);
                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully added {EntityType} with ID: {Id}",
                    typeof(TEntity).Name, GetEntityId(entry.Entity));

                return entry.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {EntityType}", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error adding {typeof(TEntity).Name}", ex);
            }
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateEntity(entity);
                var entityId = GetEntityId(entity);
                _logger.LogDebug("Updating {EntityType} with ID: {Id}", typeof(TEntity).Name, entityId);

                await ValidateBeforeUpdate(entity, cancellationToken);
                await ApplyUpdateBusinessRules(entity, cancellationToken);

                // Default: full update. Override in derived class for selective patch to avoid unintended overwrites.
                var entry = _dbSet.Update(entity);
                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated {EntityType} with ID: {Id}",
                    typeof(TEntity).Name, entityId);

                return entry.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType}", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error updating {typeof(TEntity).Name}", ex);
            }
        }

        public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateId(id);
                _logger.LogDebug("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("{EntityType} with ID {Id} not found for deletion", typeof(TEntity).Name, id);
                    return false;
                }

                await ValidateBeforeDelete(entity, cancellationToken);

                if (!TrySoftDelete(entity))
                {
                    _dbSet.Remove(entity);
                }

                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw new InvalidOperationException($"Error deleting {typeof(TEntity).Name} with ID {id}", ex);
            }
        }

        public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateId(id);
                _logger.LogDebug("Checking existence of {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

                var keyProp = GetEfKeyProperty();
                var param = Expression.Parameter(typeof(TEntity), "e");
                var propExpr = Expression.Property(param, keyProp.Name);

                object idValue = ConvertIdToPropertyType(id, keyProp.PropertyType);
                var constant = Expression.Constant(idValue, keyProp.PropertyType);

                var equal = Expression.Equal(propExpr, constant);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, param);

                return await _dbSet.AnyAsync(lambda, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw new InvalidOperationException($"Error checking existence of {typeof(TEntity).Name} with ID {id}", ex);
            }
        }

        #endregion

        #region Query Operations
        public virtual IQueryable<TEntity> Query(bool asNoTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> q = ignoreQueryFilters ? _dbSet.IgnoreQueryFilters() : _dbSet;
            return asNoTracking ? q.AsNoTracking() : q;
        }
        public virtual IQueryable<TEntity> GetQueryable() => Query();

        public virtual async Task<IEnumerable<TEntity>> GetWhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ValidatePredicate(predicate);
                _logger.LogDebug("Getting {EntityType} entities with predicate", typeof(TEntity).Name);

                var entities = await _dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                _logger.LogDebug("Retrieved {Count} {EntityType} entities with predicate", entities.Count, typeof(TEntity).Name);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} entities with predicate", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error retrieving {typeof(TEntity).Name} entities with predicate", ex);
            }
        }

        public virtual async Task<TEntity?> GetSingleWhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ValidatePredicate(predicate);
                _logger.LogDebug("Getting single {EntityType} with predicate", typeof(TEntity).Name);

                return await _dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting single {EntityType} with predicate", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error retrieving single {typeof(TEntity).Name} with predicate", ex);
            }
        }

        public virtual async Task<TEntity?> GetFirstWhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ValidatePredicate(predicate);
                _logger.LogDebug("Getting first {EntityType} with predicate", typeof(TEntity).Name);

                return await _dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first {EntityType} with predicate", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error retrieving first {typeof(TEntity).Name} with predicate", ex);
            }
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Counting {EntityType} entities", typeof(TEntity).Name);

                var query = _dbSet.AsQueryable();
                if (predicate != null)
                {
                    ValidatePredicate(predicate);
                    query = query.Where(predicate);
                }

                var count = await query.CountAsync(cancellationToken);
                _logger.LogDebug("Counted {Count} {EntityType} entities", count, typeof(TEntity).Name);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting {EntityType} entities", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error counting {typeof(TEntity).Name} entities", ex);
            }
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ValidatePredicate(predicate);
                _logger.LogDebug("Checking if any {EntityType} exists with predicate", typeof(TEntity).Name);

                return await _dbSet.AnyAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if any {EntityType} exists with predicate", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error checking if any {typeof(TEntity).Name} exists with predicate", ex);
            }
        }

        #endregion

        #region Batch Operations

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var list = entities?.ToList() ?? throw new ArgumentNullException(nameof(entities));
                if (list.Count == 0) return list;

                _logger.LogDebug("Adding {Count} {EntityType} entities", list.Count, typeof(TEntity).Name);

                foreach (var entity in list)
                {
                    ValidateEntity(entity);
                    await ValidateBeforeAdd(entity, cancellationToken);
                    await ApplyAddBusinessRules(entity, cancellationToken);
                }

                await _dbSet.AddRangeAsync(list, cancellationToken);
                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully added {Count} {EntityType} entities", list.Count, typeof(TEntity).Name);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding range of {EntityType} entities", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error adding range of {typeof(TEntity).Name} entities", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var list = entities?.ToList() ?? throw new ArgumentNullException(nameof(entities));
                if (list.Count == 0) return list;

                _logger.LogDebug("Updating {Count} {EntityType} entities", list.Count, typeof(TEntity).Name);

                foreach (var entity in list)
                {
                    ValidateEntity(entity);
                    await ValidateBeforeUpdate(entity, cancellationToken);
                    await ApplyUpdateBusinessRules(entity, cancellationToken);
                }

                _dbSet.UpdateRange(list);
                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated {Count} {EntityType} entities", list.Count, typeof(TEntity).Name);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range of {EntityType} entities", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error updating range of {typeof(TEntity).Name} entities", ex);
            }
        }

        public virtual async Task<int> DeleteRangeAsync(
            IEnumerable<TKey> ids,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var idList = ids?.ToList() ?? throw new ArgumentNullException(nameof(ids));
                if (idList.Count == 0) return 0;

                _logger.LogDebug("Deleting {Count} {EntityType} entities by IDs", idList.Count, typeof(TEntity).Name);

                var keyProp = GetEfKeyProperty();

                // Build e => idList.Contains(e.Key)
                var param = Expression.Parameter(typeof(TEntity), "e");
                var propExpr = Expression.Property(param, keyProp.Name);
                var containsMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(keyProp.PropertyType);

                // Convert id list to property type list (avoid int vs long mismatch)
                var convertedIds = ConvertIdListToPropertyType(idList, keyProp.PropertyType);
                var idsConst = Expression.Constant(convertedIds, typeof(IEnumerable<>).MakeGenericType(keyProp.PropertyType));
                var containsCall = Expression.Call(null, containsMethod, idsConst, propExpr);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(containsCall, param);

                // Load entities (kept to allow ValidateBeforeDelete + soft-delete)
                var entities = await _dbSet.Where(lambda).ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    await ValidateBeforeDelete(entity, cancellationToken);
                    if (!TrySoftDelete(entity))
                    {
                        // will be hard-delete later via RemoveRange
                    }
                }

                if (entities.Count == 0) return 0;

                // Split out to remove only those NOT soft-deleted
                var hardDeleteList = entities.Where(e => !(e is ISoftDeletable sd && sd.IsDeleted)).ToList();
                if (hardDeleteList.Count > 0)
                {
                    _dbSet.RemoveRange(hardDeleteList);
                }

                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted {Count} {EntityType} entities", entities.Count, typeof(TEntity).Name);
                return entities.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range of {EntityType} entities", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error deleting range of {typeof(TEntity).Name} entities", ex);
            }
        }

        public virtual async Task<int> DeleteWhereAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ValidatePredicate(predicate);
                _logger.LogDebug("Deleting {EntityType} entities with predicate", typeof(TEntity).Name);

                // Load to allow hook + soft-delete
                var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    await ValidateBeforeDelete(entity, cancellationToken);
                    // if soft-delete interface present -> mark deleted
                    TrySoftDelete(entity);
                }

                // Hard-delete only those not soft-deleted
                var hardDeleteList = entities.Where(e => !(e is ISoftDeletable sd && sd.IsDeleted)).ToList();
                if (hardDeleteList.Count > 0)
                {
                    _dbSet.RemoveRange(hardDeleteList);
                }

                if (AutoSaveChanges) await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted {Count} {EntityType} entities with predicate", entities.Count, typeof(TEntity).Name);
                return entities.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} entities with predicate", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error deleting {typeof(TEntity).Name} entities with predicate", ex);
            }
        }

        #endregion

        #region Transaction Support

        public virtual async Task<DomainTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Beginning transaction for {EntityType}", typeof(TEntity).Name);
                var efTx = await _context.Database.BeginTransactionAsync(cancellationToken);
                return new DbTransactionWrapper(efTx, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning transaction for {EntityType}", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error beginning transaction for {typeof(TEntity).Name}", ex);
            }
        }


        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Saving changes for {EntityType}", typeof(TEntity).Name);
                var result = await _context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("Saved {Count} changes for {EntityType}", result, typeof(TEntity).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes for {EntityType}", typeof(TEntity).Name);
                throw new InvalidOperationException($"Error saving changes for {typeof(TEntity).Name}", ex);
            }
        }

        #endregion

        #region Advanced Operations (Virtual - Can be overridden)

        public virtual async Task<TEntity?> GetByIdWithIncludesAsync(TKey id, CancellationToken cancellationToken = default)
            => await GetByIdAsync(id, cancellationToken);

        public virtual async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken cancellationToken = default)
            => await GetAllAsync(cancellationToken);

        #endregion

        #region Protected Virtual Hooks

        protected virtual Task ValidateBeforeAdd(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
        protected virtual Task ValidateBeforeUpdate(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
        protected virtual Task ValidateBeforeDelete(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task ApplyAddBusinessRules(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
        protected virtual Task ApplyUpdateBusinessRules(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual bool TrySoftDelete(TEntity entity)
        {
            if (entity is ISoftDeletable s)
            {
                s.IsDeleted = true;
                s.DeletedAt = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;
                return true;
            }
            return false;
        }

        #endregion

        #region Protected Abstracts

        protected abstract TKey GetEntityId(TEntity entity);

        #endregion

        #region Private Helpers

        private void ValidateId(TKey id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id), "ID cannot be null");

            if (typeof(TKey) == typeof(int) && Convert.ToInt32(id) <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");

            if (typeof(TKey) == typeof(long) && Convert.ToInt64(id) <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");

            if (typeof(TKey) == typeof(string) && string.IsNullOrWhiteSpace(id.ToString()))
                throw new ArgumentException("ID cannot be empty or whitespace", nameof(id));
        }

        private void ValidateEntity(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        }

        private void ValidatePredicate(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
        }

        private PropertyInfo GetEfKeyProperty()
        {
            var et = _context.Model.FindEntityType(typeof(TEntity))
                     ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not mapped.");
            var key = et.FindPrimaryKey()
                     ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} has no primary key.");
            if (key.Properties.Count != 1)
                throw new NotSupportedException($"Composite keys are not supported by {nameof(BaseRepository<TEntity, TKey>)}.");

            var prop = key.Properties[0].PropertyInfo
                       ?? typeof(TEntity).GetProperty(key.Properties[0].Name)
                       ?? throw new InvalidOperationException($"Cannot find PK PropertyInfo for {typeof(TEntity).Name}.");
            return prop;
        }

        private static object ConvertIdToPropertyType(TKey id, Type propertyType)
        {
            if (propertyType == typeof(Guid))
            {
                if (id is Guid g) return g;
                return Guid.Parse(id!.ToString()!);
            }
            if (propertyType.IsEnum)
            {
                if (id is string s) return Enum.Parse(propertyType, s, ignoreCase: true);
                return Enum.ToObject(propertyType, id);
            }
            return Convert.ChangeType(id, propertyType);
        }

        private static IEnumerable<object> ConvertIdListToPropertyType(IEnumerable<TKey> ids, Type propertyType)
        {
            foreach (var id in ids)
                yield return ConvertIdToPropertyType(id, propertyType);
        }

        #endregion
    }
    public abstract class BaseRepository<TEntity> : BaseRepository<TEntity, int>, IBaseRepository<TEntity>
        where TEntity : class
    {
        protected BaseRepository(DbContext context, ILogger logger, bool autoSaveChanges = true)
            : base(context, logger, autoSaveChanges) { }

        protected override int GetEntityId(TEntity entity)
        {
            var keyProp = typeof(TEntity).GetProperty("Id")
                         ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have an 'Id' property");
            var value = keyProp.GetValue(entity);
            return value != null ? Convert.ToInt32(value) : 0;
        }
    }

    internal sealed class DbTransactionWrapper : DomainTransaction
    {
        private readonly IDbContextTransaction _transaction;
        private readonly ILogger _logger;
        private bool _disposed = false;

        public DbTransactionWrapper(IDbContextTransaction transaction, ILogger logger)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Committing transaction");
                await _transaction.CommitAsync(cancellationToken);
                _logger.LogDebug("Transaction committed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Rolling back transaction");
                await _transaction.RollbackAsync(cancellationToken);
                _logger.LogDebug("Transaction rolled back successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction");
                throw;
            }
        }
        public void Dispose()
        {
            if (_disposed) return;
            _transaction?.Dispose();
            _disposed = true;
        }
    }
}
