using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.AuditLog;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request)
        {
            var result = await _auditLogService.GetAllAsync(request);
            return Ok(new ApiResponse<PagedResponseDto<AuditLogDto>>(true, "Audit logs retrieved successfully.", result));
        }
    }
}
