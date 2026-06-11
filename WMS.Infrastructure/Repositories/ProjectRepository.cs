using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly WMSDbContext _context;

        public ProjectRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync(string? searchTerm)
        {
            var query = _context.Projects
                .Include(p => p.Client)
                .Include(p => p.EmployeeAllocations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.ProjectName.ToLower().Contains(term));
            }

            return await query
                .OrderBy(p => p.ProjectName)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.EmployeeAllocations)
                    .ThenInclude(a => a.Employee)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public async Task<Project> AddProjectAsync(Project project, int createdByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Create Project",
                    EntityName = "Project",
                    RecordId = project.ProjectId,
                    CreatedBY = createdByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return project;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project, int updatedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Projects.Update(project);
                
                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Update Project",
                    EntityName = "Project",
                    RecordId = project.ProjectId,
                    CreatedBY = updatedByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProjectAsync(Project project, int deletedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int recordId = project.ProjectId;
                _context.Projects.Remove(project);
                
                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Delete Project",
                    EntityName = "Project",
                    RecordId = recordId,
                    CreatedBY = deletedByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EmployeeProjectAllocation> AssignEmployeeAsync(EmployeeProjectAllocation allocation, int createdByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.EmployeeProjectAllocations.AddAsync(allocation);
                await _context.SaveChangesAsync();

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Assign Employee",
                    EntityName = "EmployeeProjectAllocation",
                    RecordId = allocation.AllocationId,
                    CreatedBY = createdByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return allocation;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EmployeeProjectAllocation?> GetAllocationAsync(int allocationId)
        {
            return await _context.EmployeeProjectAllocations
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.AllocationId == allocationId);
        }

        public async Task<EmployeeProjectAllocation?> GetAllocationByEmployeeAndProjectAsync(int empId, int projectId)
        {
            return await _context.EmployeeProjectAllocations
                .FirstOrDefaultAsync(a => a.EmpId == empId && a.ProjectId == projectId);
        }

        public async Task UpdateAllocationStatusAsync(EmployeeProjectAllocation allocation, int updatedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.EmployeeProjectAllocations.Update(allocation);

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Update Allocation",
                    EntityName = "EmployeeProjectAllocation",
                    RecordId = allocation.AllocationId,
                    CreatedBY = updatedByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<EmployeeProjectAllocation>> GetAllocationHistoryAsync()
        {
            return await _context.EmployeeProjectAllocations
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsByEmployeeAsync(int employeeId)
        {
            return await _context.EmployeeProjectAllocations
                .Where(a => a.EmpId == employeeId && a.Status)
                .Include(a => a.Project)
                .Select(a => a.Project!)
                .ToListAsync();
        }

        public async Task<List<EmployeeProjectAllocation>> GetAllocationsByProjectAsync(int projectId)
        {
            return await _context.EmployeeProjectAllocations
                .Where(a => a.ProjectId == projectId)
                .Include(a => a.Employee)
                .ToListAsync();
        }
    }
}
