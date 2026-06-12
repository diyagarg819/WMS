using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Announcement
{
    public class AnnouncementListDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public List<int> TargetEmployeeIds { get; set; } = new List<int>();
    }

    public class CreateAnnouncementDto
    {
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public List<int> TargetEmployeeIds { get; set; } = new List<int>();
    }

    public class UpdateAnnouncementDto
    {
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public List<int> TargetEmployeeIds { get; set; } = new List<int>();
    }
}
