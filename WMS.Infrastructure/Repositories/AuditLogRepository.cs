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

        public async Task<IEnumerable<AuditLog>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(a => a.CreatedOn >= start);
            }

            if (endDate.HasValue)
            {
                // Add 1 day to include the entire end date if it's a date without time
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.CreatedOn <= end);
            }

            return await query
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
        }
    }
}
