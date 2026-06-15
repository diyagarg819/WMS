using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly WMSDbContext _context;

        public LeaveRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Leave>> GetAllAsync(string? status, string? searchTerm)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    (l.Employee != null && (l.Employee.FirstName.ToLower().Contains(term) || l.Employee.LastName.ToLower().Contains(term))) ||
                    l.LeaveType.ToLower().Contains(term) ||
                    (l.Reason != null && l.Reason.ToLower().Contains(term)));
            }

            return await query.OrderByDescending(l => l.AppliedOn).ToListAsync();
        }

        public async Task<List<Leave>> GetByDepartmentAsync(
            int departmentId, string? status, string? searchTerm)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Employee!.DepartmentId == departmentId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    (l.Employee != null && (l.Employee.FirstName.ToLower().Contains(term) || l.Employee.LastName.ToLower().Contains(term))) ||
                    l.LeaveType.ToLower().Contains(term) ||
                    (l.Reason != null && l.Reason.ToLower().Contains(term)));
            }

            return await query.OrderByDescending(l => l.AppliedOn).ToListAsync();
        }

        public async Task<List<Leave>> GetByEmployeeAsync(int empId, string? status, string? searchTerm)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.EmpId == empId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.LeaveType.ToLower().Contains(term) ||
                    (l.Reason != null && l.Reason.ToLower().Contains(term)));
            }

            return await query.OrderByDescending(l => l.AppliedOn).ToListAsync();
        }

        public async Task<Leave?> GetByIdAsync(int leaveId)
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<Leave> AddAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
            return leave;
        }

        public async Task UpdateStatusWithAuditAsync(Leave leave, int updatedByUserId, string actionType)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Leaves.Update(leave);

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = actionType,
                    EntityName = "Leave",
                    RecordId = leave.LeaveId,
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

        public async Task DeleteAsync(Leave leave)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
        }
    }
}
