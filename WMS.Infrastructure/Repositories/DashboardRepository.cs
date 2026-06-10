using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly WMSDbContext _context;

        public DashboardRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalEmployeeCountAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<int> GetAttendanceCountTodayAsync(string role, int employeeId, int? departmentId)
        {
            var query = _context.Attendances.Where(a => a.AttendanceDate.Date == DateTime.Today);

            if (role == "Manager" && departmentId.HasValue)
            {
                query = query.Where(a => a.Employee != null && a.Employee.DepartmentId == departmentId.Value);
            }
            else if (role == "Employee")
            {
                query = query.Where(a => a.EmpId == employeeId);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetPendingLeaveCountAsync(string role, int employeeId, int? departmentId)
        {
            var query = _context.Leaves.Where(l => l.Status == "Pending");

            if (role == "Manager" && departmentId.HasValue)
            {
                query = query.Where(l => l.Employee != null && l.Employee.DepartmentId == departmentId.Value);
            }
            else if (role == "Employee")
            {
                query = query.Where(l => l.EmpId == employeeId);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetActiveProjectCountAsync(string role, int employeeId)
        {
            if (role == "Employee")
            {
                // Count active projects where this employee has an active allocation
                return await _context.EmployeeProjectAllocations
                    .Where(a => a.EmpId == employeeId && a.Status == true && a.Project != null && a.Project.Status == "Active")
                    .Select(a => a.ProjectId)
                    .Distinct()
                    .CountAsync();
            }
            else
            {
                // Admin or Manager
                return await _context.Projects.Where(p => p.Status == "Active").CountAsync();
            }
        }

        public async Task<List<KeyValuePair<string, int>>> GetAttendanceChartDataAsync(string role, int employeeId, int? departmentId, DateTime startDate, DateTime endDate)
        {
            var query = _context.Attendances.Where(a => a.AttendanceDate.Date >= startDate.Date && a.AttendanceDate.Date <= endDate.Date);

            // Calculate how many employees are in scope to calculate "Absent" count
            var employeeQuery = _context.Employees.AsQueryable();

            if (role == "Manager" && departmentId.HasValue)
            {
                query = query.Where(a => a.Employee != null && a.Employee.DepartmentId == departmentId.Value);
                employeeQuery = employeeQuery.Where(e => e.DepartmentId == departmentId.Value);
            }
            else if (role == "Employee")
            {
                query = query.Where(a => a.EmpId == employeeId);
                employeeQuery = employeeQuery.Where(e => e.EmployeeId == employeeId);
            }

            int presentCount = await query.CountAsync();
            int totalEmployeesInScope = await employeeQuery.CountAsync();
            
            // Total possible attendances (assuming 5 work days a week, but for simplicity we take elapsed days)
            int elapsedDays = (int)(endDate.Date - startDate.Date).TotalDays + 1;
            int totalPossibleAttendances = totalEmployeesInScope * elapsedDays;

            int absentCount = totalPossibleAttendances - presentCount;
            if (absentCount < 0) absentCount = 0;

            return new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Present", presentCount),
                new KeyValuePair<string, int>("Absent", absentCount)
            };
        }

        public async Task<List<KeyValuePair<string, int>>> GetLeavePieChartDataAsync(string role, int employeeId, int? departmentId)
        {
            var query = _context.Leaves.AsQueryable();

            if (role == "Manager" && departmentId.HasValue)
            {
                query = query.Where(l => l.Employee != null && l.Employee.DepartmentId == departmentId.Value);
            }
            else if (role == "Employee")
            {
                query = query.Where(l => l.EmpId == employeeId);
            }

            var grouped = await query
                .GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new List<KeyValuePair<string, int>>();
            
            // Ensure all three statuses exist in the chart, even if 0
            int pending = grouped.FirstOrDefault(g => g.Status == "Pending")?.Count ?? 0;
            int approved = grouped.FirstOrDefault(g => g.Status == "Approved")?.Count ?? 0;
            int rejected = grouped.FirstOrDefault(g => g.Status == "Rejected")?.Count ?? 0;

            result.Add(new KeyValuePair<string, int>("Pending", pending));
            result.Add(new KeyValuePair<string, int>("Approved", approved));
            result.Add(new KeyValuePair<string, int>("Rejected", rejected));

            return result;
        }

        public async Task<List<KeyValuePair<string, int>>> GetProjectBarChartDataAsync(string role, int employeeId)
        {
            if (role == "Employee")
            {
                var query = _context.EmployeeProjectAllocations
                    .Where(a => a.EmpId == employeeId && a.Status == true && a.Project != null)
                    .Select(a => a.Project);

                var grouped = await query
                    .GroupBy(p => p!.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                int active = grouped.FirstOrDefault(g => g.Status == "Active")?.Count ?? 0;
                int completed = grouped.FirstOrDefault(g => g.Status == "Completed")?.Count ?? 0;

                return new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("Active", active),
                    new KeyValuePair<string, int>("Completed", completed)
                };
            }
            else
            {
                var grouped = await _context.Projects
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                int active = grouped.FirstOrDefault(g => g.Status == "Active")?.Count ?? 0;
                int completed = grouped.FirstOrDefault(g => g.Status == "Completed")?.Count ?? 0;

                return new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("Active", active),
                    new KeyValuePair<string, int>("Completed", completed)
                };
            }
        }

        public async Task<List<Announcement>> GetLatestAnnouncementsAsync(int count)
        {
            return await _context.Announcements
                .Include(a => a.Creator)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedOn)
                .Take(count)
                .ToListAsync();
        }
    }
}
