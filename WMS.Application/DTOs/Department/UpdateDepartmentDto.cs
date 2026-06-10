using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        [Required(ErrorMessage = "DepartmentName is required.")]
        [MaxLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
