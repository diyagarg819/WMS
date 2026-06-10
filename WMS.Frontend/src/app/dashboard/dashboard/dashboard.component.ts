import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard.service';
import { DashboardResponse, DashboardChart } from '../../shared/models/dashboard.model';
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
  
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;
  private chartInstance: Chart | null = null;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  ngAfterViewInit(): void {
    // Chart will be rendered after data is loaded and view is checked
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.dashboardService.getDashboardData().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.dashboardData = res.data;
          // Render chart slightly after data load to ensure canvas is in DOM
          setTimeout(() => this.renderChart(), 0);
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

  renderChart(): void {
    if (!this.chartCanvas || !this.dashboardData || !this.dashboardData.charts || this.dashboardData.charts.length === 0) {
      return;
    }

    const chartData = this.dashboardData.charts[0]; // Display the first chart
    
    if (this.chartInstance) {
      this.chartInstance.destroy();
    }

    const ctx = this.chartCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    // Use theme variables for chart styling
    const style = getComputedStyle(document.body);
    const textColor = style.getPropertyValue('--text-color').trim() || '#1e293b';
    const gridColor = style.getPropertyValue('--border-color').trim() || '#e2e8f0';

    this.chartInstance = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: chartData.labels,
        datasets: [{
          label: chartData.chartName,
          data: chartData.data,
          backgroundColor: 'rgba(67, 56, 202, 0.7)',
          borderColor: '#4338ca',
          borderWidth: 1,
          borderRadius: 4
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: { color: textColor }
          }
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
      }
    });
  }
}
