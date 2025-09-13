using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(UserAddress entity, CancellationToken ct)
        {
           return _context.UserAddresses.AddAsync(entity, ct).AsTask();
        }

        public async Task ClearDefaultAsync(int userId, CancellationToken ct)
        {
            await _context.UserAddresses
                .Where
                (u => u.UserId == userId && u.IsDefault == true)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(u => u.IsDefault, false), ct);
        }
        public async Task<int> HardDeleteAsync(int id, CancellationToken ct)
        {
            var e = await _context.UserAddresses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e == null) return 0;
            _context.UserAddresses.Remove(e);
            return await _context.SaveChangesAsync(ct);
        }

        public Task<UserAddress?> FindAsync(int id, int userId, CancellationToken ct)
        {
            return _context.UserAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(ua => ua.Id == id && ua.UserId == userId, ct);
        }

        public IQueryable<UserAddress> GetFilteredQueryable(UserAddressFilterParams filter, bool includeUser = false)
        {
            var q = _context.UserAddresses.AsQueryable();
            q = q
                .AsNoTracking()
                .Where(ua => !ua.IsDeleted)
                .Include(ua => ua.User);
            if (filter.IsDeleted == true)
                q = q
                    .IgnoreQueryFilters()
                    .Where(x => x.IsDeleted == true);
            if(includeUser)
                q = q.Include(q => q.User);
            if (filter.UserId.HasValue) 
                q = q.Where(x => x.UserId == filter.UserId.Value);
            if(filter.IsDefault.HasValue) 
                q = q.Where(x => x.IsDefault == filter.IsDefault.Value);
            if(!string.IsNullOrWhiteSpace(filter.CountryCode)) 
                q = q.Where(x => x.CountryCode == filter.CountryCode);
            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                var city = filter.City!.Trim();
                q = q.Where(x => x.City == city);
            }
            if (!string.IsNullOrWhiteSpace(filter.Ward))
            {
                var ward = filter.Ward!.Trim();
                q = q.Where(x => x.Ward == ward);
            }

            if (filter.CreatedFrom.HasValue)
                q = q.Where(x => x.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                q = q.Where(x => x.CreatedAt <= filter.CreatedTo.Value);

            if (filter.UpdatedFrom.HasValue)
                q = q.Where(x => x.UpdatedAt >= filter.UpdatedFrom.Value);

            if (filter.UpdatedTo.HasValue)
                q = q.Where(x => x.UpdatedAt <= filter.UpdatedTo.Value);

            return q;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);


        public Task UpdateAsync(UserAddress entity, CancellationToken ct)
        {
            _context.UserAddresses.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<int> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _context.UserAddresses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
            Throw.IfNull(entity,nameof(UserAddress), id);

            if (entity!.IsDeleted) 
                return 0;

            entity.IsDeleted = true;
            entity.IsDefault = false;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            var entry = _context.Entry(entity);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeletedAt).IsModified = true;
            entry.Property(x => x.IsDefault).IsModified = true;
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<UserAddress?> FindByIdAsync(int id, CancellationToken ct)
        {
            return await _context.UserAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(ua => ua.Id == id, ct);
        }

        public async Task<int> UnsetDefaultForUserAsync(int userId, int skipId, CancellationToken ct)
        {
            return await _context.UserAddresses
                .Where(x => x.UserId == userId
                            && x.Id != skipId
                            && x.IsDefault == false
                            && x.IsDeleted == true)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false), ct);
        }

        public EntityEntry<UserAddress> Entry(UserAddress entity) => _context.Entry(entity);

        public IQueryable<UserAddress> Query()
        {
            return _context.UserAddresses
                .AsNoTracking()
                .Where(w => !w.IsDeleted)
                .Include(i => i.User);
        }

        public IQueryable<UserAddress> QueryIgnoreFilters()
        {
            return _context.UserAddresses
                .AsNoTracking()
                .Include(i => i.User);
        }
    }
}
