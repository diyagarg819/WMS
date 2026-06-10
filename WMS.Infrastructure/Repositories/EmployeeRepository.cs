using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Employee database operations.
    /// Only this class touches the database for employee-related queries.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly WMSDbContext _context;

        public EmployeeRepository(WMSDbContext context)
        {
            _context = context;
        }

        // Paginated, searchable, sortable list of all employees
        public async Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm, string? sortBy, string? sortDirection)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .AsQueryable();

            // Apply search filter — searches across first name, last name, email, and phone
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term) ||
                    e.PhoneNumber.Contains(term));
            }

            // Get total count before pagination (for page metadata)
            int totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDirection);

            // Apply pagination
            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }

        // Get a single employee by ID — includes Department and Role
        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        // Check if an email is already in use (optionally exclude a specific employee for updates)
        public async Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null)
        {
            var query = _context.Employees.AsNoTracking().Where(e => e.Email == email);

            if (excludeEmployeeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeEmployeeId.Value);

            return await query.AnyAsync();
        }

        // Add a new employee record
        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        // Update an existing employee record
        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        // Soft delete — set Status to 'Inactive' and add an AuditLog entry
        public async Task DeleteAsync(Employee employee, int deletedByUserId)
        {
            employee.Status = "Inactive";
            employee.UpdatedOn = DateTime.Now;
            _context.Employees.Update(employee);

            // Write an audit log entry for the soft delete
            var auditLog = new AuditLog
            {
                Action = "SoftDelete",
                EntityName = "Employee",
                RecordId = employee.EmployeeId,
                CreatedBY = deletedByUserId,
                CreatedOn = DateTime.Now
            };
            await _context.AuditLogs.AddAsync(auditLog);

            await _context.SaveChangesAsync();
        }

        // Get employees within a specific department (Manager team-scoped view)
        public async Task<(List<Employee> Employees, int TotalCount)> GetByDepartmentAsync(
            int departmentId, int pageNumber, int pageSize, string? searchTerm)
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

            int totalCount = await query.CountAsync();

            var employees = await query
                .OrderBy(e => e.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }

        // Apply dynamic sorting based on the sort field name
        private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string? sortDirection)
        {
            bool isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLower() switch
            {
                "firstname" => isDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
                "lastname" => isDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "department" => isDescending ? query.OrderByDescending(e => e.Department!.DepartmentName) : query.OrderBy(e => e.Department!.DepartmentName),
                "status" => isDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
                "doj" => isDescending ? query.OrderByDescending(e => e.DOJ) : query.OrderBy(e => e.DOJ),
                _ => query.OrderBy(e => e.EmployeeId) // Default sort by ID
            };
        }
    }
}
