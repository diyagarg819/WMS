using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project
{
    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; } = string.Empty;

        public int? ClientId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";
    }
}
