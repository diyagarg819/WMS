using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents a company-wide announcement created by an Admin.
    /// Active announcements are displayed on the dashboard for all users.
    /// </summary>
    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnnouncementId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        // The employee (Admin) who created this announcement
        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Employee? Creator { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Whether this announcement is currently visible to users
        public bool IsActive { get; set; } = true;
    }
}
