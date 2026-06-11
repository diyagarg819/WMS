using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Infrastructure.Data;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public RoleController(WMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            return Ok(new ApiResponse<object>(true, "Roles retrieved", roles));
        }
    }
}
