import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../shared/services/project.service';
import { ProjectAllocation } from '../../shared/models/project.model';

@Component({
  selector: 'app-allocation-management',
  templateUrl: './allocation-management.component.html',
  styleUrls: ['./allocation-management.component.scss'],
  standalone: false
})
export class AllocationManagementComponent implements OnInit {
  allocations: ProjectAllocation[] = [];
  filteredAllocations: ProjectAllocation[] = [];
  displayedColumns: string[] = ['employeeName', 'projectName', 'assignedOn', 'status', 'createdBY'];
  isLoading = true;

  // Filter options
  projectsList: string[] = [];
  statusList: string[] = ['All', 'Active', 'Inactive'];
  appliedByList: string[] = [];

  // Selected filters
  selectedProject: string = 'All';
  selectedStatus: string = 'All';
  selectedAppliedBy: string = 'All';

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.isLoading = true;
    this.projectService.getAllocationHistory().subscribe({
      next: (res) => {
        if (res.data) {
          this.allocations = res.data;
          this.filteredAllocations = res.data;
          this.extractFilterOptions();
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load history', err);
        this.isLoading = false;
      }
    });
  }

  extractFilterOptions(): void {
    const projects = new Set(this.allocations.map(a => a.projectName));
    this.projectsList = ['All', ...Array.from(projects).sort()];

    const appliedBy = new Set(this.allocations.map(a => a.createdBY));
    this.appliedByList = ['All', ...Array.from(appliedBy).sort()];
  }

  applyFilters(): void {
    this.filteredAllocations = this.allocations.filter(a => {
      const matchProject = this.selectedProject === 'All' || a.projectName === this.selectedProject;
      
      let matchStatus = true;
      if (this.selectedStatus === 'Active') {
        matchStatus = a.status === true;
      } else if (this.selectedStatus === 'Inactive') {
        matchStatus = a.status === false;
      }

      const matchAppliedBy = this.selectedAppliedBy === 'All' || a.createdBY === this.selectedAppliedBy;

      return matchProject && matchStatus && matchAppliedBy;
    });
  }
}
