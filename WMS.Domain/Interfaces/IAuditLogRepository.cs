using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
