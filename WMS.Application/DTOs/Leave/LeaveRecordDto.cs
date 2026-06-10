namespace WMS.Application.DTOs.Leave
{
    /// <summary>
    /// DTO for returning leave request details.
    /// </summary>
    public class LeaveRecordDto
    {
        public int LeaveId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedOn { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
    }
}
