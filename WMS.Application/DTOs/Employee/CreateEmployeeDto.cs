using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Request body for creating a new employee — Admin only.
    /// Fields match the Employee table schema exactly.
    /// </summary>
    public class CreateEmployeeDto
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

        /// <summary>
        /// Login username for the new employee.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Login password for the new employee (will be hashed before storage).
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
