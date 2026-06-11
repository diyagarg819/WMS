using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly WMSDbContext _context;

        public EmployeeRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(string? searchTerm)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .Where(e => e.Status != "Inactive")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                bool isNumeric = int.TryParse(term, out int searchId);

                query = query.Where(e =>
                    (isNumeric && e.EmployeeId == searchId) ||
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term) ||
                    (e.PhoneNumber != null && e.PhoneNumber.ToLower().Contains(term)) ||
                    (e.Department != null && e.Department.DepartmentName.ToLower().Contains(term)) ||
                    (e.Role != null && e.Role.RoleName.ToLower().Contains(term)));
            }

            return await query.OrderBy(e => e.EmployeeId).ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null)
        {
            var query = _context.Employees.AsNoTracking().Where(e => e.Email == email);

            if (excludeEmployeeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeEmployeeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Employee> AddAsync(Employee employee, int createdByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Create Employee",
                    EntityName = "Employee",
                    RecordId = employee.EmployeeId,
                    CreatedBY = createdByUserId,
                    CreatedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return employee;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Employee employee, int updatedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Employees.Update(employee);

                await _context.AuditLogs.AddAsync(new AuditLog
                {
                    Action = "Update Employee",
                    EntityName = "Employee",
                    RecordId = employee.EmployeeId,
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

        public async Task DeleteAsync(Employee employee, int deletedByUserId)
        {
            employee.Status = "Inactive";
            employee.UpdatedOn = DateTime.Now;
            _context.Employees.Update(employee);

            await _context.AuditLogs.AddAsync(new AuditLog
            {
                Action = "SoftDelete",
                EntityName = "Employee",
                RecordId = employee.EmployeeId,
                CreatedBY = deletedByUserId,
                CreatedOn = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<Employee>> GetByDepartmentAsync(int departmentId, string? searchTerm)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .Where(e => e.DepartmentId == departmentId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            return await query.OrderBy(e => e.FirstName).ToListAsync();
        }
    }
}
