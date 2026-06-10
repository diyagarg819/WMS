using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for UserLogin database operations.
    /// This is the only place that touches the database for auth-related data.
    /// </summary>
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly WMSDbContext _context;

        public UserLoginRepository(WMSDbContext context)
        {
            _context = context;
        }

        // Find a user by username — include Role so we can read the role name for JWT claims
        public async Task<UserLogin?> GetByUsernameAsync(string username)
        {
            var user = await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        // Find a user by their primary key
        public async Task<UserLogin?> GetByIdAsync(int userId)
        {
            var user = await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user;
        }

        // Save changes to an existing user login record
        public async Task UpdateAsync(UserLogin userLogin)
        {
            _context.UserLogins.Update(userLogin);
            await _context.SaveChangesAsync();
        }

        // Insert a new user login record into the database
        public async Task AddAsync(UserLogin userLogin)
        {
            await _context.UserLogins.AddAsync(userLogin);
            await _context.SaveChangesAsync();
        }

        // Check if a username is already in use
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.UserLogins
                .AsNoTracking()
                .AnyAsync(u => u.Username == username);
        }

        // Check if an employee record exists — needed when creating a login for an employee
        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AsNoTracking()
                .AnyAsync(e => e.EmployeeId == employeeId);
        }
    }
}
