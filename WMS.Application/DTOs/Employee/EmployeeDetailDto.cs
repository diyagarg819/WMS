namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Full detail DTO for the employee detail panel — all readable fields.
    /// Never exposes internal fields like PasswordHash.
    /// </summary>
    public class EmployeeDetailDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DOJ { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Username { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
