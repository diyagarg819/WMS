using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly WMSDbContext _context;

        public AuditLogRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<AuditLog> records, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.AuditLogs.AsQueryable();

            int totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(a => a.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalCount);
        }
    }
}
