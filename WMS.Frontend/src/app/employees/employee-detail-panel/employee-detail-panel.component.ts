import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Employee } from '../../shared/models/employee.model';
import { EmployeeService } from '../../shared/services/employee.service';

@Component({
  selector: 'app-employee-detail-panel',
  templateUrl: './employee-detail-panel.component.html',
  styleUrls: ['./employee-detail-panel.component.scss'],
  standalone: false
})
export class EmployeeDetailPanelComponent {
  @Input() employee: Employee | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Employee>();
  @Output() deleted = new EventEmitter<boolean>();

  showDeleteConfirm = false;
  isDeleting = false;

  constructor(private employeeService: EmployeeService) {}

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
