namespace WMS.Application.DTOs.Dashboard
{
    public class AnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string CreatorName { get; set; } = string.Empty;
    }
}
