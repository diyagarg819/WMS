using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IProjectRepository
    {
        // Project CRUD
        Task<(List<Project> Projects, int TotalCount)> GetAllProjectsAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> AddProjectAsync(Project project, int createdByUserId);
        Task UpdateProjectAsync(Project project, int updatedByUserId);
        Task DeleteProjectAsync(Project project, int deletedByUserId);

        // Allocations
        Task<EmployeeProjectAllocation> AssignEmployeeAsync(EmployeeProjectAllocation allocation, int createdByUserId);
        Task<EmployeeProjectAllocation?> GetAllocationAsync(int allocationId);
        Task<EmployeeProjectAllocation?> GetAllocationByEmployeeAndProjectAsync(int empId, int projectId);
        Task UpdateAllocationStatusAsync(EmployeeProjectAllocation allocation, int updatedByUserId);
    }
}
