import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ProjectService } from '../../shared/services/project.service';
import { EmployeeService } from '../../shared/services/employee.service';
import { AuthService } from '../../shared/services/auth.service';
import { ProjectRecord, ProjectAllocation } from '../../shared/models/project.model';
import { Employee } from '../../shared/models/employee.model';
import { Role } from '../../shared/enums/role.enum';

@Component({
  selector: 'app-project-detail-panel',
  templateUrl: './project-detail-panel.component.html',
  styleUrls: ['./project-detail-panel.component.scss'],
  standalone: false
})
export class ProjectDetailPanelComponent implements OnInit {
  @Input() project: ProjectRecord | null = null;
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  isSaving = false;
  errorMessage = '';
  
  showAssignForm = false;
  employees: Employee[] = [];
  selectedEmpId: number | null = null;

  constructor(
    private projectService: ProjectService,
    private employeeService: EmployeeService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    if (this.canManageAllocations()) {
      this.loadEmployees();
    }
  }

  loadEmployees(): void {
    // Load first 100 employees for the dropdown
    this.employeeService.getEmployees(1, 100).subscribe(res => {
      if (res.data) {
        this.employees = res.data.data;
      }
    });
  }

  canManageAllocations(): boolean {
    const role = this.authService.getRole();
    return role === Role.Admin || role === Role.Manager;
  }

  toggleAssignForm(): void {
    this.showAssignForm = !this.showAssignForm;
    this.selectedEmpId = null;
    this.errorMessage = '';
  }

  assignEmployee(): void {
    if (!this.project || !this.selectedEmpId) return;

    this.isSaving = true;
    this.errorMessage = '';

    this.projectService.assignEmployee(this.project.projectId, { empId: this.selectedEmpId }).subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: res.message || 'Employee assigned successfully.' });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to assign employee.';
        this.isSaving = false;
      }
    });
  }

  removeAllocation(allocationId: number): void {
    if (!confirm('Are you sure you want to remove this employee from the project?')) return;

    this.isSaving = true;
    this.errorMessage = '';

    this.projectService.removeEmployee(allocationId).subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: res.message || 'Employee removed successfully.' });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to remove employee.';
        this.isSaving = false;
      }
    });
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
