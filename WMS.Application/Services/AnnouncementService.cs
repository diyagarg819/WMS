using WMS.Application.Common;
using WMS.Application.DTOs.Announcement;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _repository;

        public AnnouncementService(IAnnouncementRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AnnouncementListDto>> GetAllAsync(SearchRequestDto request, bool? isActive, int currentUserId, string role)
        {
            var records = await _repository.GetAllAsync(request.SearchTerm, isActive);

            // Filter for employee role
            if (role == "Employee")
            {
                records = records.Where(a => 
                    !a.TargetEmployees.Any() || // Global announcement
                    a.TargetEmployees.Any(te => te.EmployeeId == currentUserId) // Targeted to them
                ).ToList();
            }

            return records.Select(a => new AnnouncementListDto
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Message = a.Message,
                IsActive = a.IsActive,
                CreatedOn = a.CreatedOn,
                CreatorName = a.Creator != null ? $"{a.Creator.FirstName} {a.Creator.LastName}" : "System",
                TargetEmployeeIds = a.TargetEmployees.Select(te => te.EmployeeId).ToList()
            }).ToList();
        }

        public async Task<(bool Success, string Message)> CreateAsync(CreateAnnouncementDto request, int createdByUserId)
        {
            var announcement = new Announcement
            {
                Title = request.Title,
                Message = request.Message,
                CreatedBy = createdByUserId,
                CreatedOn = DateTime.Now,
                IsActive = true
            };

            foreach (var empId in request.TargetEmployeeIds)
            {
                announcement.TargetEmployees.Add(new AnnouncementEmployee { EmployeeId = empId });
            }

            await _repository.AddAsync(announcement);
            return (true, "Announcement created.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateAnnouncementDto request)
        {
            var announcement = await _repository.GetByIdAsync(id);
            if (announcement == null)
                return (false, "Announcement not found.");

            announcement.Title = request.Title;
            announcement.Message = request.Message;
            announcement.IsActive = request.IsActive;

            // Clear and rebuild target employees
            announcement.TargetEmployees.Clear();
            foreach (var empId in request.TargetEmployeeIds)
            {
                announcement.TargetEmployees.Add(new AnnouncementEmployee { EmployeeId = empId });
            }

            await _repository.UpdateAsync(announcement);
            return (true, "Announcement updated.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var announcement = await _repository.GetByIdAsync(id);
            if (announcement == null)
                return (false, "Announcement not found.");

            await _repository.DeleteAsync(id);
            return (true, "Announcement deleted.");
        }
    }
}
