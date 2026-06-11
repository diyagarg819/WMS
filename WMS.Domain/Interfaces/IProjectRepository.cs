using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllProjectsAsync(string? searchTerm);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> AddProjectAsync(Project project, int createdByUserId);
        Task UpdateProjectAsync(Project project, int updatedByUserId);
        Task DeleteProjectAsync(Project project, int deletedByUserId);

        Task<EmployeeProjectAllocation> AssignEmployeeAsync(EmployeeProjectAllocation allocation, int createdByUserId);
        Task<EmployeeProjectAllocation?> GetAllocationAsync(int allocationId);
        Task<EmployeeProjectAllocation?> GetAllocationByEmployeeAndProjectAsync(int empId, int projectId);
        Task UpdateAllocationStatusAsync(EmployeeProjectAllocation allocation, int updatedByUserId);
        Task<List<EmployeeProjectAllocation>> GetAllocationHistoryAsync();
        Task<List<Project>> GetProjectsByEmployeeAsync(int employeeId);
        Task<List<EmployeeProjectAllocation>> GetAllocationsByProjectAsync(int projectId);
    }
}
