import { Component, OnInit, ElementRef, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard.service';
import { DashboardResponse, DashboardChart } from '../../shared/models/dashboard.model';
import { AuthService } from '../../shared/services/auth.service';
import { Role } from '../../shared/enums/role.enum';
import { ThemeService } from '../../shared/services/theme.service';
import { Subscription } from 'rxjs';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent implements OnInit, AfterViewInit {
  dashboardData: DashboardResponse | null = null;
  isLoading = true;
  errorMessage = '';
  
  @ViewChild('leaveChartCanvas') leaveChartCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('projectChartCanvas') projectChartCanvas?: ElementRef<HTMLCanvasElement>;

  private leaveChartInstance: Chart | null = null;
  private projectChartInstance: Chart | null = null;
  private themeSub: Subscription | null = null;
  private isDark = false;

  constructor(
    private dashboardService: DashboardService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private themeService: ThemeService
  ) {}

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  get isEmployee(): boolean {
    return this.authService.getRole() === Role.Employee;
  }

  ngOnInit(): void {
    this.loadDashboard();
    this.themeSub = this.themeService.isDarkMode$.subscribe(isDark => {
      this.isDark = isDark;
      if (this.dashboardData) {
        // slight delay to let DOM styles update before re-rendering
        setTimeout(() => this.renderAllCharts(), 50);
      }
    });
  }

  ngAfterViewInit(): void {
    if (this.dashboardData) {
      this.renderAllCharts();
    }
  }

  ngOnDestroy(): void {
    if (this.themeSub) {
      this.themeSub.unsubscribe();
    }
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.dashboardService.getDashboardData().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success && res.data) {
          this.dashboardData = res.data;
          // Force view update so ViewChildren resolve before rendering
          this.cdr.detectChanges();
          // Adding a short timeout ensures the DOM has actually painted the canvases
          setTimeout(() => this.renderAllCharts(), 100);
        } else {
          this.errorMessage = res.message || 'Failed to load dashboard.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Error communicating with server.';
      }
    });
  }

  renderAllCharts(): void {
    if (!this.dashboardData || !this.dashboardData.charts) return;

    const textColor = this.isDark ? '#e2e8f0' : '#1e293b';
    const gridColor = this.isDark ? '#334155' : '#e2e8f0';

    // 1. Leave Statistics Chart or Monthly Attendance
    const leaveData = this.dashboardData.charts.find(c => c.chartName === 'LeaveStatus');
    const attendanceData = this.dashboardData.charts.find(c => c.chartName === 'MonthlyAttendance');

    if (leaveData && this.leaveChartCanvas) {
      if (this.leaveChartInstance) this.leaveChartInstance.destroy();
      const ctx = this.leaveChartCanvas.nativeElement.getContext('2d');
      if (ctx) {
        this.leaveChartInstance = new Chart(ctx, {
          type: 'doughnut',
          data: {
            labels: leaveData.labels,
            datasets: [{
              label: 'Leaves',
              data: leaveData.data,
              backgroundColor: [
                'rgba(245, 158, 11, 0.8)', // Pending
                'rgba(16, 185, 129, 0.8)', // Approved
                'rgba(239, 68, 68, 0.8)'   // Rejected
              ],
              borderWidth: 1
            }]
          },
          options: this.getPieChartOptions(textColor)
        });
      }
    } else if (attendanceData && this.leaveChartCanvas) {
      if (this.leaveChartInstance) this.leaveChartInstance.destroy();
      const ctx = this.leaveChartCanvas.nativeElement.getContext('2d');
      if (ctx) {
        this.leaveChartInstance = new Chart(ctx, {
          type: 'bar',
          data: {
            labels: attendanceData.labels,
            datasets: [{
              label: 'Attendance',
              data: attendanceData.data,
              backgroundColor: 'rgba(16, 185, 129, 0.7)',
              borderColor: '#10b981',
              borderWidth: 1,
              borderRadius: 4
            }]
          },
          options: this.getBarChartOptions(textColor, gridColor)
        });
      }
    }

    // 2. Project Status Chart
    const projectData = this.dashboardData.charts.find(c => c.chartName === 'ProjectStatus');
    if (projectData && this.projectChartCanvas) {
      if (this.projectChartInstance) this.projectChartInstance.destroy();
      const ctx = this.projectChartCanvas.nativeElement.getContext('2d');
      if (ctx) {
        this.projectChartInstance = new Chart(ctx, {
          type: 'bar',
          data: {
            labels: projectData.labels,
            datasets: [{
              label: 'Projects',
              data: projectData.data,
              backgroundColor: 'rgba(14, 165, 233, 0.7)',
              borderColor: '#0ea5e9',
              borderWidth: 1,
              borderRadius: 4
            }]
          },
          options: this.getBarChartOptions(textColor, gridColor)
        });
      }
    }
  }

  private getBarChartOptions(textColor: string, gridColor: string): any {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { labels: { color: textColor } }
      },
      scales: {
        x: {
          ticks: { color: textColor },
          grid: { color: gridColor, display: false }
        },
        y: {
          beginAtZero: true,
          ticks: { color: textColor, precision: 0 },
          grid: { color: gridColor }
        }
      }
    };
  }

  private getPieChartOptions(textColor: string): any {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'bottom', labels: { color: textColor } }
      }
    };
  }
}
