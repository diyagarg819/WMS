using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Request body for updating an employee — Admin can update all fields.
    /// Employee can only update their own PhoneNumber via the /my-profile endpoint.
    /// </summary>
    public class UpdateEmployeeDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(80)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(1)]
        public string? Gender { get; set; }

        [Required]
        [WMS.Application.Common.MinimumAge(18)]
        public DateTime DOB { get; set; }

        [Required]
        public DateTime DOJ { get; set; }

        public int? DepartmentId { get; set; }

        public int? RoleId { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// Login username — can be changed by Admin.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// New password — optional. If provided, password will be updated (hashed).
        /// Leave empty to keep the current password.
        /// </summary>
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }
    }
}
