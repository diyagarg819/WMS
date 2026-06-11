using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly WMSDbContext _context;

        public DepartmentRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync(string? searchTerm)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(d => d.DepartmentName.ToLower().Contains(term));
            }

            return await query
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == name.ToLower());
        }

        public async Task<Department> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
