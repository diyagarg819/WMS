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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchRequestDto request)
        {
            var result = await _projectService.GetAllProjectsAsync(request);
            return Ok(new ApiResponse<List<ProjectDto>>(true, "Projects retrieved successfully.", result));
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto request)
        {
            int userId = GetCurrentUserId();
            var result = await _projectService.CreateProjectAsync(request, userId);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProjectId }, new ApiResponse<ProjectDto>(true, result.Message, result.Data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{projectId}/employees")]
        public async Task<IActionResult> GetEmployeesByProject(int projectId)
        {
            var result = await _projectService.GetEmployeesByProjectAsync(projectId);
            return Ok(new ApiResponse<List<ProjectAllocationDto>>(true, "Employees retrieved.", result));
        }
    }
}
