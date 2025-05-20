using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null");

                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new ArgumentException("Email cannot be null or empty", nameof(user.Email));

                var isEmailUnique = await IsEmailUniqueAsync(user.Email, null, cancellationToken);
                if (!isEmailUnique)
                    throw new InvalidOperationException($"Email '{user.Email}' is already in use");

                user.CreatedAt = DateTime.UtcNow;
                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding user", ex);
            }
        }

        public async Task<User> ChangeActiveAsync(int id, bool isActive, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found");

                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error changing active status for user {id}", ex);
            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting user {id}", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving all users", ex);
            }
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));

                var user = await _context.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user == null)
                    throw new KeyNotFoundException($"User with email {email} not found");

                return user;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                throw new InvalidOperationException($"Error retrieving user by email {email}", ex);
            }
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found");

                return user;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                throw new InvalidOperationException($"Error retrieving user {id}", ex);
            }
        }

        public async Task<IEnumerable<User>> GetFilteredAsync(UserFilterParams filterParams, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Users
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
                throw new InvalidOperationException("Error retrieving filtered users", ex);
            }
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, UserFilterParams filterParams, CancellationToken cancellationToken)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _context.Users
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
                throw new InvalidOperationException("Error retrieving paged users", ex);
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            var query = _context.Users.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);

            return !await query.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user), "User cannot be null");

                if (user.Id <= 0)
                    throw new ArgumentException("User ID must be greater than zero", nameof(user.Id));

                var existingUser = await _context.Users.FindAsync(new object[] { user.Id }, cancellationToken);
                if (existingUser == null)
                    throw new KeyNotFoundException($"User with ID {user.Id} not found");

                if (!string.IsNullOrWhiteSpace(user.Email) && user.Email != existingUser.Email)
                {
                    var isEmailUnique = await IsEmailUniqueAsync(user.Email, user.Id, cancellationToken);
                    if (!isEmailUnique)
                        throw new InvalidOperationException($"Email '{user.Email}' is already in use");
                }

                // Update properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;

                if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                    existingUser.PasswordHash = user.PasswordHash;

                existingUser.Phone = user.Phone;
                existingUser.AvatarUrl = user.AvatarUrl;
                existingUser.RoleId = user.RoleId;
                existingUser.Gender = user.Gender;
                existingUser.DateOfBirth = user.DateOfBirth;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTime.UtcNow;
                existingUser.UpdatedBy = user.UpdatedBy;

                await _context.SaveChangesAsync(cancellationToken);
                return existingUser;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException) && !(ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Error updating user {user.Id}", ex);
            }
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
