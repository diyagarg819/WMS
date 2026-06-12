using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly WMSDbContext _context;

        public AnnouncementRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Announcement>> GetAllAsync(string? searchTerm, bool? isActive)
        {
            var query = _context.Announcements
                .Include(a => a.Creator)
                .Include(a => a.TargetEmployees)
                .AsQueryable();

            if (isActive.HasValue)
                query = query.Where(a => a.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(a => a.Title.Contains(searchTerm) || a.Message.Contains(searchTerm));

            return await query
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            return await _context.Announcements
                .Include(a => a.TargetEmployees)
                .FirstOrDefaultAsync(a => a.AnnouncementId == id);
        }

        public async Task AddAsync(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
            }
        }
    }
}
