using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> GetAllAsync(string? searchTerm, bool? isActive);
        Task<Announcement?> GetByIdAsync(int id);
        Task AddAsync(Announcement announcement);
        Task UpdateAsync(Announcement announcement);
        Task DeleteAsync(int id);
    }
}
