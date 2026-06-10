using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project
{
    public class AssignEmployeeDto
    {
        [Required]
        public int EmpId { get; set; }
    }
}
