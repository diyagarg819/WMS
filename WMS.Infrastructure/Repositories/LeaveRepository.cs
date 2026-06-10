using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Leave database operations.
    /// Handles EF Core transactions for audit logging during approval/rejection.
    /// </summary>
    public class LeaveRepository : ILeaveRepository
    {
        private readonly WMSDbContext _context;

        public LeaveRepository(WMSDbContext context)
        {
            _context = context;
        }

        // Get paginated leaves across all employees (Admin view)
        public async Task<(List<Leave> Leaves, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? status, string? searchTerm)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.Employee!.FirstName.ToLower().Contains(term) ||
                    l.Employee!.LastName.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var leaves = await query
                .OrderByDescending(l => l.AppliedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (leaves, totalCount);
        }

        // Get paginated leaves for a specific department (Manager view)
        public async Task<(List<Leave> Leaves, int TotalCount)> GetByDepartmentAsync(
            int departmentId, int pageNumber, int pageSize, string? status, string? searchTerm)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Employee!.DepartmentId == departmentId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.Employee!.FirstName.ToLower().Contains(term) ||
                    l.Employee!.LastName.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var leaves = await query
                .OrderByDescending(l => l.AppliedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (leaves, totalCount);
        }

        // Get paginated leaves for a specific employee (Employee view)
        public async Task<(List<Leave> Leaves, int TotalCount)> GetByEmployeeAsync(
            int empId, int pageNumber, int pageSize, string? status)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.EmpId == empId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l => l.Status == status);
            }

            int totalCount = await query.CountAsync();

            var leaves = await query
                .OrderByDescending(l => l.AppliedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (leaves, totalCount);
        }

        // Get a single leave request by ID
        public async Task<Leave?> GetByIdAsync(int leaveId)
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        // Add a new leave request
        public async Task<Leave> AddAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
            return leave;
        }

        // Update leave status (Approve/Reject) and insert an AuditLog in a single transaction
        public async Task UpdateStatusWithAuditAsync(Leave leave, int updatedByUserId, string actionType)
        {
            // Use a transaction when updating Leave and writing the audit log together
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Leaves.Update(leave);

                // Write an audit log entry for this action
                var auditLog = new AuditLog
                {
                    Action = actionType,
                    EntityName = "Leave",
                    RecordId = leave.LeaveId,
                    CreatedBY = updatedByUserId,
                    CreatedOn = DateTime.Now
                };
                
                await _context.AuditLogs.AddAsync(auditLog);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Delete a pending leave request
        public async Task DeleteAsync(Leave leave)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
        }
    }
}
