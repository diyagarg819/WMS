import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AnnouncementService } from '../../shared/services/announcement.service';
import { EmployeeService } from '../../shared/services/employee.service';
import { AnnouncementRecord } from '../../shared/models/announcement.model';
import { Employee } from '../../shared/models/employee.model';

@Component({
  selector: 'app-announcement-form-panel',
  templateUrl: './announcement-form-panel.component.html',
  styleUrls: ['./announcement-form-panel.component.scss'],
  standalone: false
})
export class AnnouncementFormPanelComponent implements OnInit {
  @Input() announcement: AnnouncementRecord | null = null;
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  form: FormGroup;
  isSaving = false;
  errorMessage = '';
  employees: Employee[] = [];
  selectedEmployeeIds: number[] = [];

  get isEditMode(): boolean {
    return this.announcement !== null;
  }

  constructor(
    private fb: FormBuilder,
    private announcementService: AnnouncementService,
    private employeeService: EmployeeService
  ) {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      message: ['', Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadEmployees();

    if (this.announcement) {
      this.form.patchValue({
        title: this.announcement.title,
        message: this.announcement.message,
        isActive: this.announcement.isActive
      });
      if (this.announcement.targetEmployeeIds && this.announcement.targetEmployeeIds.length > 0) {
        this.selectedEmployeeIds = [...this.announcement.targetEmployeeIds];
      }
    }
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employees = res.data;
          // If editing an existing announcement and targetEmployeeIds are not set (e.g. legacy data or empty array usually means all if it was previously targetAudience='All' but wait, the backend now expects targetEmployeeIds to have all Ids if it's 'All' or empty if global)
          // Wait, backend logic for global announcement is TargetEmployeeIds being empty list!
          // Actually, let's just use what's in the announcement.targetEmployeeIds array.
          if (this.announcement) {
             if (!this.announcement.targetEmployeeIds || this.announcement.targetEmployeeIds.length === 0) {
                // If it's a global announcement, select all employees in the UI.
                this.selectedEmployeeIds = this.employees.map(e => e.employeeId);
             }
          }
        }
      }
    });
  }

  get isAllSelected(): boolean {
    return this.employees.length > 0 && this.selectedEmployeeIds.length === this.employees.length;
  }

  toggleAll(selectAll: boolean): void {
    if (selectAll) {
      this.selectedEmployeeIds = this.employees.map(e => e.employeeId);
    } else {
      this.selectedEmployeeIds = [];
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.selectedEmployeeIds.length === 0) {
      this.errorMessage = this.selectedEmployeeIds.length === 0 ? 'Select at least one employee.' : '';
      return;
    }
    this.isSaving = true;
    this.errorMessage = '';

    // If all are selected, we send an empty array to indicate global announcement (or we can send all Ids. The backend handles empty list as Global).
    // Let's send an empty list if all are selected to save space.
    const targetEmployeeIds = this.selectedEmployeeIds.length === this.employees.length
      ? [] : this.selectedEmployeeIds;

    if (this.isEditMode) {
      this.announcementService.update(this.announcement!.announcementId, {
        title: this.form.value.title, message: this.form.value.message,
        isActive: this.form.value.isActive, targetEmployeeIds
      }).subscribe({
        next: (res) => this.actionComplete.emit({ success: true, message: res.message || 'Updated.' }),
        error: (err) => { this.errorMessage = err.error?.message || 'Failed.'; this.isSaving = false; }
      });
    } else {
      this.announcementService.create({
        title: this.form.value.title, message: this.form.value.message, targetEmployeeIds
      }).subscribe({
        next: (res) => this.actionComplete.emit({ success: true, message: res.message || 'Created.' }),
        error: (err) => { this.errorMessage = err.error?.message || 'Failed.'; this.isSaving = false; }
      });
    }
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
