using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project
{
    public class AssignEmployeeDto
    {
        [Required]
        public int EmpId { get; set; }

        // Used by POST /api/projectallocation (standalone endpoint)
        public int ProjectId { get; set; }
    }
}
