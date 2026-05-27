using E_LaptopShop.Domain.Constants;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _appContext;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
            : base(context, logger)  
        {
            _appContext = context;
        }

        public override async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _appContext.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UserMessages.ErrorRetrievingUser, id);
                throw new InvalidOperationException(string.Format(UserMessages.ErrorRetrievingUser, id), ex);
            }
        }

        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _appContext.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UserMessages.ErrorRetrievingAllUsers);
                throw new InvalidOperationException(UserMessages.ErrorRetrievingAllUsers, ex);
            }
        }

        protected override async Task ValidateBeforeAdd(User entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(entity.Email))
                throw new ArgumentException(UserMessages.EmailRequired);

            var isEmailUnique = await IsEmailUniqueAsync(entity.Email, null, cancellationToken);
            if (!isEmailUnique)
                throw new InvalidOperationException(string.Format(UserMessages.EmailAlreadyInUse, entity.Email));
        }

        protected override async Task ApplyAddBusinessRules(User entity, CancellationToken cancellationToken)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await Task.CompletedTask;
        }

        protected override async Task ValidateBeforeUpdate(User entity, CancellationToken cancellationToken)
        {
            if (entity.Id <= 0)
                throw new ArgumentException(UserMessages.UserIdMustBePositive);

            // Check email uniqueness nếu email thay đổi
            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                var existingUser = await _appContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == entity.Id, cancellationToken);

                if (existingUser != null && entity.Email != existingUser.Email)
                {
                    var isEmailUnique = await IsEmailUniqueAsync(entity.Email, entity.Id, cancellationToken);
                    if (!isEmailUnique)
                        throw new InvalidOperationException(string.Format(UserMessages.EmailAlreadyInUse, entity.Email));
                }
            }
            await Task.CompletedTask;
        }

        protected override async Task ApplyUpdateBusinessRules(User entity, CancellationToken cancellationToken)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            await Task.CompletedTask;
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException(UserMessages.EmailRequired, nameof(email));

                var user = await _appContext.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user == null)
                    throw new KeyNotFoundException(string.Format(UserMessages.UserNotFoundByEmail, email));

                return user;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                throw new InvalidOperationException(string.Format(UserMessages.ErrorRetrievingByEmail, email), ex);
            }
        }

        public async Task<User> ChangeActiveAsync(int id, bool isActive, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _appContext.Users.FindAsync(new object[] { id }, cancellationToken);
                if (user == null)
                    throw new KeyNotFoundException(string.Format(UserMessages.UserNotFoundById, id));

                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                await _appContext.SaveChangesAsync(cancellationToken);
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(UserMessages.ErrorChangingActiveStatus, id), ex);
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(UserMessages.EmailRequired, nameof(email));

            var query = _appContext.Users.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);

            return !await query.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetFilteredAsync(UserFilterParams filterParams, CancellationToken cancellationToken)
        {
            try
            {
                var query = _appContext.Users
                    .Include(u => u.Role)
                    .AsNoTracking();

                query = ApplyFilters(query, filterParams);
                query = ApplySorting(query, filterParams.SortBy, filterParams.IsAscending);

                if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
                {
                    query = ApplySearch(query, filterParams.SearchTerm);
                }

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(UserMessages.ErrorRetrievingFiltered, ex);
            }
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, UserFilterParams filterParams, CancellationToken cancellationToken)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _appContext.Users
                    .Include(u => u.Role)
                    .AsNoTracking();

                query = ApplyFilters(query, filterParams);

                if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
                {
                    query = ApplySearch(query, filterParams.SearchTerm);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                query = ApplySorting(query, filterParams.SortBy, filterParams.IsAscending);

                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(UserMessages.ErrorRetrievingPaged, ex);
            }
        }

        public Task IncrementLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ResetLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        private IQueryable<User> ApplyFilters(IQueryable<User> query, UserFilterParams filterParams)
        {
            if (filterParams == null)
                return query;

            if (filterParams.Id.HasValue)
                query = query.Where(u => u.Id == filterParams.Id.Value);

            if (!string.IsNullOrWhiteSpace(filterParams.FirstName))
                query = query.Where(u => u.FirstName.Contains(filterParams.FirstName));

            if (!string.IsNullOrWhiteSpace(filterParams.LastName))
                query = query.Where(u => u.LastName.Contains(filterParams.LastName));

            if (!string.IsNullOrWhiteSpace(filterParams.Email))
                query = query.Where(u => u.Email.Contains(filterParams.Email));

            if (!string.IsNullOrWhiteSpace(filterParams.Phone))
                query = query.Where(u => u.Phone != null && u.Phone.Contains(filterParams.Phone));

            if (filterParams.RoleId.HasValue)
                query = query.Where(u => u.RoleId == filterParams.RoleId.Value);

            if (filterParams.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filterParams.IsActive.Value);

            if (filterParams.EmailConfirmed.HasValue)
                query = query.Where(u => u.EmailConfirmed == filterParams.EmailConfirmed.Value);

            if (!string.IsNullOrWhiteSpace(filterParams.Gender))
                query = query.Where(u => u.Gender != null && u.Gender.Contains(filterParams.Gender));

            if (filterParams.CreatedAtFrom.HasValue)
                query = query.Where(u => u.CreatedAt >= filterParams.CreatedAtFrom.Value);

            if (filterParams.CreatedAtTo.HasValue)
                query = query.Where(u => u.CreatedAt <= filterParams.CreatedAtTo.Value);

            return query;
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query.OrderByDescending(u => u.CreatedAt);

            return sortBy.ToLower() switch
            {
                "id" => isAscending ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id),
                "firstname" => isAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName),
                "lastname" => isAscending ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName),
                "email" => isAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "roleid" => isAscending ? query.OrderBy(u => u.RoleId) : query.OrderByDescending(u => u.RoleId),
                "isactive" => isAscending ? query.OrderBy(u => u.IsActive) : query.OrderByDescending(u => u.IsActive),
                "createdat" => isAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };
        }

        private IQueryable<User> ApplySearch(IQueryable<User> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var lowerSearchTerm = searchTerm.ToLower();

            return query.Where(u =>
                (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchTerm)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchTerm)) ||
                (u.Email != null && u.Email.ToLower().Contains(lowerSearchTerm)) ||
                (u.Phone != null && u.Phone.ToLower().Contains(lowerSearchTerm)) ||
                (u.Gender != null && u.Gender.ToLower().Contains(lowerSearchTerm))
            );
        }
    }
}
