using WMS.Application.Common;
using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Services
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementListDto>> GetAllAsync(SearchRequestDto request, bool? isActive, int currentUserId, string role);
        Task<(bool Success, string Message)> CreateAsync(CreateAnnouncementDto request, int createdByUserId);
        Task<(bool Success, string Message)> UpdateAsync(int id, UpdateAnnouncementDto request);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
