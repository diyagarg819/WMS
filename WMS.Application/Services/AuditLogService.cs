using WMS.Application.DTOs.AuditLog;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public interface IAuditLogService
    {
        Task<List<AuditLogDto>> GetAllAsync(DateTime? startDate, DateTime? endDate);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<AuditLogDto>> GetAllAsync(DateTime? startDate, DateTime? endDate)
        {
            var records = await _auditLogRepository.GetAllAsync(startDate, endDate);

            return records.Select(a => new AuditLogDto
            {
                AuditId = a.AuditId,
                EntityName = a.EntityName,
                RecordId = a.RecordId,
                Action = a.Action,
                CreatedBy = a.CreatedBY,
                CreatedOn = a.CreatedOn
            }).ToList();
        }
    }
}
