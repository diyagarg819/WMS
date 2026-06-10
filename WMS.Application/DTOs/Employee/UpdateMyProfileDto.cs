using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Request body for when an Employee updates their own profile.
    /// Only PhoneNumber can be changed — Email, Role, Department, and Status are Admin-only.
    /// </summary>
    public class UpdateMyProfileDto
    {
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
