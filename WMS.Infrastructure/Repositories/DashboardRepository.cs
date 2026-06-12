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

        public async Task<int> GetLeavesTakenCountAsync(string role, int employeeId, int? departmentId)
        {
            var query = _context.Leaves.Where(l => l.Status == "Approved");

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

        public async Task<int> GetTotalProjectsCountAsync(string role, int employeeId)
        {
            if (role == "Employee")
            {
                return await _context.EmployeeProjectAllocations
                    .Where(a => a.EmpId == employeeId && a.Status == true && a.Project != null)
                    .Select(a => a.ProjectId)
                    .Distinct()
                    .CountAsync();
            }
            else
            {
                return await _context.Projects.CountAsync();
            }
        }

        public async Task<int> GetAllocatedEmployeesCountAsync(string role, int employeeId)
        {
            if (role == "Employee")
            {
                return 1; // It's just themselves
            }
            else
            {
                return await _context.EmployeeProjectAllocations
                    .Where(a => a.Status == true)
                    .Select(a => a.EmpId)
                    .Distinct()
                    .CountAsync();
            }
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

        public async Task<List<KeyValuePair<string, int>>> GetMonthlyAttendanceBarChartDataAsync(string role, int employeeId, int? departmentId)
        {
            var query = _context.Attendances.AsQueryable();

            if (role == "Manager" && departmentId.HasValue)
            {
                query = query.Where(a => a.Employee != null && a.Employee.DepartmentId == departmentId.Value);
            }
            else if (role == "Employee")
            {
                query = query.Where(a => a.EmpId == employeeId);
            }

            var today = DateTime.Today;
            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            
            // Generate last 6 months data
            var result = new List<KeyValuePair<string, int>>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = currentMonthStart.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var count = await query.Where(a => a.AttendanceDate >= monthStart && a.AttendanceDate <= monthEnd).CountAsync();
                result.Add(new KeyValuePair<string, int>(monthStart.ToString("MMM yyyy"), count));
            }

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
