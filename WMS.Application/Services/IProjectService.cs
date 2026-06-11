using WMS.Application.Common;
using WMS.Application.DTOs.Project;

namespace WMS.Application.Services
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllProjectsAsync(SearchRequestDto request);
        Task<(bool Success, string Message, ProjectDto? Data)> GetProjectByIdAsync(int id);
        
        Task<(bool Success, string Message, ProjectDto? Data)> CreateProjectAsync(CreateProjectDto request, int userId);
        Task<(bool Success, string Message, ProjectDto? Data)> UpdateProjectAsync(int id, UpdateProjectDto request, int userId);
        Task<(bool Success, string Message)> DeleteProjectAsync(int id, int userId);

        Task<(bool Success, string Message, ProjectAllocationDto? Data)> AssignEmployeeAsync(int projectId, AssignEmployeeDto request, int userId, string userName);
        Task<(bool Success, string Message)> RemoveEmployeeAsync(int allocationId, int userId, string userName);

        Task<List<ProjectAllocationDto>> GetAllocationHistoryAsync();
        Task<List<ProjectDto>> GetProjectsByEmployeeAsync(int employeeId);
        Task<List<ProjectAllocationDto>> GetEmployeesByProjectAsync(int projectId);
    }
}
