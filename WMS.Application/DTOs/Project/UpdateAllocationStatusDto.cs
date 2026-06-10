using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project
{
    public class UpdateAllocationStatusDto
    {
        [Required]
        public bool Status { get; set; }
    }
}
