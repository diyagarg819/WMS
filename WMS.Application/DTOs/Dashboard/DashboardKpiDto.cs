namespace WMS.Application.DTOs.Dashboard
{
    public class DashboardKpiDto
    {
        public int TotalEmployees { get; set; }
        public int AttendanceCountToday { get; set; }
        public int PendingLeaveCount { get; set; }
        public int LeavesTaken { get; set; }
        public int ActiveProjectCount { get; set; }
        public int TotalProjects { get; set; }
        public int AllocatedEmployees { get; set; }
    }
}
