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
      if (this.announcement.targetAudience !== 'All') {
        this.selectedEmployeeIds = this.announcement.targetAudience
          .split(',').map(id => parseInt(id.trim(), 10)).filter(id => !isNaN(id));
      }
    }
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employees = res.data;
          if (this.announcement && this.announcement.targetAudience === 'All') {
            this.selectedEmployeeIds = this.employees.map(e => e.employeeId);
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

    const targetAudience = this.selectedEmployeeIds.length === this.employees.length
      ? 'All' : this.selectedEmployeeIds.join(',');

    if (this.isEditMode) {
      this.announcementService.update(this.announcement!.announcementId, {
        title: this.form.value.title, message: this.form.value.message,
        isActive: this.form.value.isActive, targetAudience
      }).subscribe({
        next: (res) => this.actionComplete.emit({ success: true, message: res.message || 'Updated.' }),
        error: (err) => { this.errorMessage = err.error?.message || 'Failed.'; this.isSaving = false; }
      });
    } else {
      this.announcementService.create({
        title: this.form.value.title, message: this.form.value.message, targetAudience
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
