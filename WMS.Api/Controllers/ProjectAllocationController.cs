using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Project;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectAllocationController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectAllocationController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignEmployee([FromBody] AssignEmployeeDto request)
        {
            if (request.ProjectId <= 0)
                return BadRequest(new ApiResponse<object?>(false, "ProjectId is required."));

            int userId = GetCurrentUserId();
            string userName = GetCurrentUserName();

            var result = await _projectService.AssignEmployeeAsync(request.ProjectId, request, userId, userName);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<ProjectAllocationDto>(true, result.Message, result.Data!));
        }

        [HttpPut("remove/{allocationId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveEmployee(int allocationId)
        {
            int userId = GetCurrentUserId();
            string userName = GetCurrentUserName();

            var result = await _projectService.RemoveEmployeeAsync(allocationId, userId, userName);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<object?>(true, result.Message));
        }

        [HttpGet("history")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetHistory()
        {
            var result = await _projectService.GetAllocationHistoryAsync();
            return Ok(new ApiResponse<List<ProjectAllocationDto>>(true, "History retrieved.", result));
        }

        [HttpGet("/api/employees/{employeeId}/projects")]
        [Authorize]
        public async Task<IActionResult> GetProjectsByEmployee(int employeeId)
        {
            var result = await _projectService.GetProjectsByEmployeeAsync(employeeId);
            return Ok(new ApiResponse<List<ProjectDto>>(true, "Projects retrieved.", result));
        }
    }
}
