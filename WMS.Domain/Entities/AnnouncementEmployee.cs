using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    public class AnnouncementEmployee
    {
        public int AnnouncementId { get; set; }
        public Announcement Announcement { get; set; } = null!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
