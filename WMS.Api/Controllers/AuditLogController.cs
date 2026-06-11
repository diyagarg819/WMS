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
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _auditLogService.GetAllAsync(startDate, endDate);
            return Ok(new ApiResponse<List<AuditLogDto>>(true, "Audit logs retrieved.", result));
        }
    }
}
