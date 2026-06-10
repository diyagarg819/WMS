namespace WMS.Application.DTOs.Dashboard
{
    public class DashboardChartDto
    {
        public string ChartName { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>();
    }
}
