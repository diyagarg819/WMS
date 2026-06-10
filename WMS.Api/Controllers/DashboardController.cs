using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // Extract Identity and Role securely from the User Token
            int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
            string role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";

            var result = await _dashboardService.GetDashboardDataAsync(userId, role);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object?>(false, result.Message));
            }

            return Ok(new ApiResponse<DashboardResponseDto>(true, result.Message, result.Data!));
        }
    }
}
