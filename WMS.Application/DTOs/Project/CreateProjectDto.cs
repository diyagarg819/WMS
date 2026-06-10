using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project
{
    public class CreateProjectDto
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
