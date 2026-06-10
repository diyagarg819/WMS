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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
        }

        private string GetCurrentUserName()
        {
            // Fallback to empty string if GivenName doesn't exist
            return User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request)
        {
            var result = await _projectService.GetAllProjectsAsync(request);
            return Ok(new ApiResponse<PagedResponseDto<ProjectDto>>(true, "Projects retrieved successfully.", result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);

            if (!result.Success)
                return NotFound(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<ProjectDto>(true, result.Message, result.Data!));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto request)
        {
            int userId = GetCurrentUserId();
            var result = await _projectService.CreateProjectAsync(request, userId);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProjectId }, new ApiResponse<ProjectDto>(true, result.Message, result.Data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto request)
        {
            int userId = GetCurrentUserId();
            var result = await _projectService.UpdateProjectAsync(id, request, userId);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<ProjectDto>(true, result.Message, result.Data!));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var result = await _projectService.DeleteProjectAsync(id, userId);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<object?>(true, result.Message));
        }

        // --- Allocations ---

        [HttpPost("{id}/assign")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignEmployee(int id, [FromBody] AssignEmployeeDto request)
        {
            int userId = GetCurrentUserId();
            string userName = GetCurrentUserName();

            var result = await _projectService.AssignEmployeeAsync(id, request, userId, userName);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<ProjectAllocationDto>(true, result.Message, result.Data!));
        }

        [HttpPost("allocation/{allocationId}/remove")]
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

        [HttpPut("allocation/{allocationId}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateAllocationStatus(int allocationId, [FromBody] UpdateAllocationStatusDto request)
        {
            int userId = GetCurrentUserId();
            string userName = GetCurrentUserName();

            var result = await _projectService.UpdateAllocationStatusAsync(allocationId, request, userId, userName);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<ProjectAllocationDto>(true, result.Message, result.Data!));
        }
    }
}
