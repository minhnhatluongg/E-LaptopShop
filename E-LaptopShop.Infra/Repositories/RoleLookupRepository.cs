using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra.Repositories
{
    public sealed class RoleLookupRepository : IRoleLookup
    {
        private readonly ApplicationDbContext _db;
        private static readonly ConcurrentDictionary<string, (int Id, string Name)> _cache = new();

        public RoleLookupRepository(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<int> GetIdByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var key = Normalize(code);
            if (_cache.TryGetValue(key, out var hit)) return hit.Id;

            var loaded = await LoadByCodeAsync(key, cancellationToken);
            _cache.TryAdd(key, loaded);
            return loaded.Id;
        }
        public async Task<string> GetNameByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var key = Normalize(code);
            if (_cache.TryGetValue(key, out var hit)) return hit.Name;

            var loaded = await LoadByCodeAsync(key, cancellationToken);
            _cache.TryAdd(key, loaded);
            return loaded.Name;
        }
        public void Invalidate(string code)
        {
            _cache.TryRemove(Normalize(code), out _);
        }

        private static string Normalize(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Role code is required.", nameof(code));
            return code.Trim().ToUpperInvariant();
        }

        private async Task<(int Id, string Name)> LoadByCodeAsync(string codeUpper, CancellationToken ct)
        {
            var row = await _db.Set<Role>()
                .AsNoTracking()
                .Where(r => r.Code == codeUpper && r.IsActive)
                .Select(r => new { r.Id, r.Name })
                .SingleOrDefaultAsync(ct);

            if (row is null)
                throw new KeyNotFoundException($"Role with code '{codeUpper}' not found.");
            return (row.Id, row.Name);
        }
    }
}
