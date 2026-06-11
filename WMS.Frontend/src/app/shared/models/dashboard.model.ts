export interface DashboardKpi {
  totalEmployees: number;
  attendanceCountToday: number;
  pendingLeaveCount: number;
  leavesTaken: number;
  activeProjectCount: number;
  totalProjects: number;
  allocatedEmployees: number;
}

export interface DashboardChart {
  chartName: string;
  labels: string[];
  data: number[];
}

export interface Announcement {
  announcementId: number;
  title: string;
  message: string;
  createdOn: string;
  creatorName: string;
}

export interface DashboardResponse {
  kpis: DashboardKpi;
  charts: DashboardChart[];
  announcements: Announcement[];
}
