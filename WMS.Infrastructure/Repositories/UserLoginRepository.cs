using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly WMSDbContext _context;

        public UserLoginRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<UserLogin?> GetByUsernameAsync(string username)
        {
            return await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserLogin?> GetByIdAsync(int userId)
        {
            return await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateAsync(UserLogin userLogin)
        {
            _context.UserLogins.Update(userLogin);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(UserLogin userLogin)
        {
            await _context.UserLogins.AddAsync(userLogin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.UserLogins
                .AsNoTracking()
                .AnyAsync(u => u.Username == username);
        }
    }
}
