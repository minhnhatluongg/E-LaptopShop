using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Role> AddAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role), "Role cannot be null");
                }
                role.IsActive = true; 
                if (string.IsNullOrWhiteSpace(role.Name))
                {
                    throw new ArgumentException("Role name cannot be null or empty", nameof(role.Name));
                }
                await _context.Roles.AddAsync(role, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return role;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding role", ex);
            }

        }

        public async Task<Role> ChangeActiveAsync(int id, bool isActive, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
                }
                var role = await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
                if (role == null)
                {
                    throw new KeyNotFoundException($"Role with ID {id} not found");
                }
                role.IsActive = isActive;
                await _context.SaveChangesAsync(cancellationToken);
                return role;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Error changing role active status", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error changing role active status", ex);

            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
            }
            var role = await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found");
            }
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            return id;
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Roles
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving all roles", ex);
            }
        }

        public async Task<IEnumerable<Role>> GetFilteredAsync(int? id, string? name, bool? isActive, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                if (id.HasValue)
                    query = query.Where(r => r.Id == id);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var searchName = name.Trim().ToLower();
                    query = query.Where(r => r.Name.ToLower().Contains(searchName));
                }

                if (isActive.HasValue)
                    query = query.Where(r => r.IsActive == isActive);

                var result = await query.ToListAsync(cancellationToken);
                
                // Log the SQL query and results for debugging
                System.Diagnostics.Debug.WriteLine($"Filter parameters - Id: {id}, Name: {name}, IsActive: {isActive}");
                System.Diagnostics.Debug.WriteLine($"Found {result.Count()} results");
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving filtered roles. Parameters: Id={id}, Name={name}, IsActive={isActive}", ex);
            }
        }

        public async Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving role with ID {id}", ex);
            }
        }

        public async Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role), "Role cannot be null");
                }
                var existingRole = _context.Roles.Find(role.Id);
                if (existingRole == null)
                {
                    throw new InvalidOperationException($"Role with ID {role.Id} not found");
                }
                //Cập nhật tất cả các giá trị thuộc tính
                //_context.Entry(existingRole).CurrentValues.SetValues(role);
                //Cập nhật từng thuộc tính
                existingRole.Name = role.Name;
                existingRole.IsActive = role.IsActive;

                _context.Roles.Update(existingRole);
                await _context.SaveChangesAsync(cancellationToken);

                return existingRole;

            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Error updating role", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating role", ex);
            }
        }
    }
}
