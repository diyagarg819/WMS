import { Component, OnInit, ElementRef, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard.service';
import { DashboardResponse, DashboardChart } from '../../shared/models/dashboard.model';
import { AuthService } from '../../shared/services/auth.service';
import { Role } from '../../shared/enums/role.enum';
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

  constructor(
    private dashboardService: DashboardService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  ngOnInit(): void {
    this.loadDashboard();
  }

  ngAfterViewInit(): void {
    // Attempt render if data came extremely fast
    if (this.dashboardData) {
      this.renderAllCharts();
    }
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.dashboardService.getDashboardData().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.dashboardData = res.data;
          // Force view update so ViewChildren resolve before rendering
          this.cdr.detectChanges();
          // Adding a short timeout ensures the DOM has actually painted the canvases
          setTimeout(() => this.renderAllCharts(), 100);
        } else {
          this.errorMessage = res.message || 'Failed to load dashboard.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Error communicating with server.';
        this.isLoading = false;
      }
    });
  }

  renderAllCharts(): void {
    if (!this.dashboardData || !this.dashboardData.charts) return;

    const style = getComputedStyle(document.body);
    const textColor = style.getPropertyValue('--text-color').trim() || '#1e293b';
    const gridColor = style.getPropertyValue('--border-color').trim() || '#e2e8f0';

    // 1. Leave Statistics Chart
    const leaveData = this.dashboardData.charts.find(c => c.chartName === 'LeaveStatus');
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
