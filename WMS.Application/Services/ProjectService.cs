using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Project;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IProjectRepository projectRepository, 
            IEmployeeRepository employeeRepository,
            ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync(SearchRequestDto request)
        {
            var records = await _projectRepository.GetAllProjectsAsync(
                request.SearchTerm);

            return records.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string Message, ProjectDto? Data)> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return (false, "Project not found.", null);
            }

            return (true, "Project found.", MapToDto(project));
        }

        public async Task<(bool Success, string Message, ProjectDto? Data)> CreateProjectAsync(CreateProjectDto request, int userId)
        {
            var project = new Project
            {
                ProjectName = request.ProjectName,
                ClientId = request.ClientId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status
            };

            var created = await _projectRepository.AddProjectAsync(project, userId);
            
            _logger.LogInformation("Project created: {ProjectId} by user {UserId}", created.ProjectId, userId);

            return (true, "Project created successfully.", MapToDto(created));
        }

        public async Task<(bool Success, string Message, ProjectDto? Data)> UpdateProjectAsync(int id, UpdateProjectDto request, int userId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return (false, "Project not found.", null);
            }

            project.ProjectName = request.ProjectName;
            project.ClientId = request.ClientId;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.Status = request.Status;

            await _projectRepository.UpdateProjectAsync(project, userId);

            _logger.LogInformation("Project updated: {ProjectId} by user {UserId}", project.ProjectId, userId);

            return (true, "Project updated successfully.", MapToDto(project));
        }

        public async Task<(bool Success, string Message)> DeleteProjectAsync(int id, int userId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return (false, "Project not found.");
            }

            if (project.EmployeeAllocations != null && project.EmployeeAllocations.Any(a => a.Status))
            {
                return (false, "Cannot delete a project with active employee allocations.");
            }

            await _projectRepository.DeleteProjectAsync(project, userId);

            _logger.LogInformation("Project deleted: {ProjectId} by user {UserId}", id, userId);

            return (true, "Project deleted successfully.");
        }

        public async Task<(bool Success, string Message, ProjectAllocationDto? Data)> AssignEmployeeAsync(int projectId, AssignEmployeeDto request, int userId, string userName)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (project == null) return (false, "Project not found.", null);

            var employee = await _employeeRepository.GetByIdAsync(request.EmpId);
            if (employee == null) return (false, "Employee not found.", null);

            var existing = await _projectRepository.GetAllocationByEmployeeAndProjectAsync(request.EmpId, projectId);
            if (existing != null)
            {
                if (existing.Status)
                    return (false, "Employee is already actively assigned to this project.", null);
                else
                    return (false, "Employee has a prior inactive assignment to this project. Please update the existing allocation status.", null);
            }

            var allocation = new EmployeeProjectAllocation
            {
                EmpId = request.EmpId,
                ProjectId = projectId,
                AssignedOn = DateTime.Now,
                CreateDate = DateTime.Now,
                CreatedBY = userName,
                Status = true // Active by default
            };

            var created = await _projectRepository.AssignEmployeeAsync(allocation, userId);
            var updatedAllocation = await _projectRepository.GetAllocationAsync(created.AllocationId);

            _logger.LogInformation("Employee {EmpId} assigned to Project {ProjectId}", request.EmpId, projectId);

            return (true, "Employee assigned to project.", MapAllocationToDto(updatedAllocation!));
        }

        public async Task<(bool Success, string Message)> RemoveEmployeeAsync(int allocationId, int userId, string userName)
        {
            var allocation = await _projectRepository.GetAllocationAsync(allocationId);
            if (allocation == null) return (false, "Allocation not found.");

            // Hard delete or inactive? Requirements say "remove", so we can mark inactive or delete.
            // But EmployeeProjectAllocation doesn't have an IProjectRepository.DeleteAllocation method.
            // Setting Status = false instead.
            allocation.Status = false;
            allocation.UpdatedBy = userName;
            allocation.UpdatedDate = DateTime.Now;

            await _projectRepository.UpdateAllocationStatusAsync(allocation, userId);

            _logger.LogInformation("Allocation {AllocationId} marked inactive by {UserId}", allocationId, userId);

            return (true, "Employee removed from project successfully.");
        }


        private ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ClientId = project.ClientId,
                ClientName = project.Client?.ClientName,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Allocations = project.EmployeeAllocations?.Where(a => a.Status).Select(MapAllocationToDto).ToList() ?? new List<ProjectAllocationDto>()
            };
        }

        private ProjectAllocationDto MapAllocationToDto(EmployeeProjectAllocation allocation)
        {
            return new ProjectAllocationDto
            {
                AllocationId = allocation.AllocationId,
                EmpId = allocation.EmpId,
                EmployeeName = allocation.Employee != null ? $"{allocation.Employee.FirstName} {allocation.Employee.LastName}" : string.Empty,
                ProjectId = allocation.ProjectId,
                ProjectName = allocation.Project?.ProjectName ?? string.Empty,
                AssignedOn = allocation.AssignedOn,
                Status = allocation.Status,
                CreatedBY = allocation.CreatedBY
            };
        }

        public async Task<List<ProjectAllocationDto>> GetAllocationHistoryAsync()
        {
            var allocations = await _projectRepository.GetAllocationHistoryAsync();
            return allocations.Select(MapAllocationToDto).ToList();
        }

        public async Task<List<ProjectDto>> GetProjectsByEmployeeAsync(int employeeId)
        {
            var projects = await _projectRepository.GetProjectsByEmployeeAsync(employeeId);
            return projects.Select(MapToDto).ToList();
        }

        public async Task<List<ProjectAllocationDto>> GetEmployeesByProjectAsync(int projectId)
        {
            var allocations = await _projectRepository.GetAllocationsByProjectAsync(projectId);
            return allocations.Where(a => a.Status).Select(MapAllocationToDto).ToList();
        }
    }
}
