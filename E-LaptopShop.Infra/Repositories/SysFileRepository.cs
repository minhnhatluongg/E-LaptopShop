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
    public class SysFileRepository : ISysFileRepository
    {
        private readonly ApplicationDbContext _context;

        public SysFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SysFile> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var sysFile = await _context.SysFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (sysFile == null)
                throw new KeyNotFoundException($"SysFile with ID {id} not found");

            return sysFile;
        }

        public async Task<int> AddAsync(SysFile sysFile, CancellationToken cancellationToken = default)
        {
            await _context.SysFiles.AddAsync(sysFile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return sysFile.Id;
        }

        public async Task UpdateAsync(SysFile sysFile, CancellationToken cancellationToken = default)
        {
            _context.Entry(sysFile).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var sysFile = await _context.SysFiles.FindAsync(new object[] { id }, cancellationToken);
            if (sysFile != null)
            {
                _context.SysFiles.Remove(sysFile);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<SysFile>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            return await _context.SysFiles
                .AsNoTracking()
                .Where(f => ids.Contains(f.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
