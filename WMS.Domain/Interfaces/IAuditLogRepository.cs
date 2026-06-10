using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<(IEnumerable<AuditLog> records, int totalCount)> GetAllAsync(int pageNumber, int pageSize);
    }
}
