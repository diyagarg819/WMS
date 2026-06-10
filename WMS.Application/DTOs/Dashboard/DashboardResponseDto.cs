namespace WMS.Application.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        public DashboardKpiDto Kpis { get; set; } = new DashboardKpiDto();
        public List<DashboardChartDto> Charts { get; set; } = new List<DashboardChartDto>();
        public List<AnnouncementDto> Announcements { get; set; } = new List<AnnouncementDto>();
    }
}
