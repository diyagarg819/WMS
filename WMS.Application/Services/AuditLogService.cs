using WMS.Application.Common;
using WMS.Application.DTOs.AuditLog;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public interface IAuditLogService
    {
        Task<PagedResponseDto<AuditLogDto>> GetAllAsync(PagedRequestDto request);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<PagedResponseDto<AuditLogDto>> GetAllAsync(PagedRequestDto request)
        {
            var (records, totalCount) = await _auditLogRepository.GetAllAsync(request.PageNumber, request.PageSize);

            var dtoList = records.Select(a => new AuditLogDto
            {
                AuditId = a.AuditId,
                EntityName = a.EntityName,
                RecordId = a.RecordId,
                Action = a.Action,
                CreatedBy = a.CreatedBY,
                CreatedOn = a.CreatedOn
            }).ToList();

            return new PagedResponseDto<AuditLogDto>
            {
                Data = dtoList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
