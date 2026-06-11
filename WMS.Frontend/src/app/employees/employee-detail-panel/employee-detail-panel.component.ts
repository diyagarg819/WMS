import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { Employee } from '../../shared/models/employee.model';
import { EmployeeService } from '../../shared/services/employee.service';
import { AuthService } from '../../shared/services/auth.service';
import { Role } from '../../shared/enums/role.enum';

@Component({
  selector: 'app-employee-detail-panel',
  templateUrl: './employee-detail-panel.component.html',
  styleUrls: ['./employee-detail-panel.component.scss'],
  standalone: false
})
export class EmployeeDetailPanelComponent implements OnChanges {
  @Input() employee: Employee | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Employee>();
  @Output() deleted = new EventEmitter<boolean>();

  showDeleteConfirm = false;
  isDeleting = false;
  isLoading = false;

  constructor(private employeeService: EmployeeService, private authService: AuthService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employee'] && changes['employee'].currentValue) {
      this.fetchFullDetails(changes['employee'].currentValue.employeeId);
    }
  }

  fetchFullDetails(id: number): void {
    this.isLoading = true;
    this.employeeService.getEmployeeById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employee = res.data;
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load full employee details', err);
        this.isLoading = false;
      }
    });
  }

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  onEdit(): void {
    if (this.employee) {
      this.edit.emit(this.employee);
    }
  }

  onDeleteConfirm(): void {
    if (!this.employee) return;
    
    this.isDeleting = true;
    this.employeeService.deleteEmployee(this.employee.employeeId).subscribe({
      next: () => {
        this.isDeleting = false;
        this.deleted.emit(true);
      },
      error: () => {
        this.isDeleting = false;
        this.deleted.emit(false);
      }
    });
  }
}
