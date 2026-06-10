import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee } from '../../shared/models/employee.model';
import { EmployeeService } from '../../shared/services/employee.service';

@Component({
  selector: 'app-employee-form-panel',
  templateUrl: './employee-form-panel.component.html',
  styleUrls: ['./employee-form-panel.component.scss'],
  standalone: false
})
export class EmployeeFormPanelComponent implements OnInit, OnChanges {
  @Input() mode: 'add' | 'edit' = 'add';
  @Input() employee: Employee | null = null;
  
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<boolean>();

  employeeForm!: FormGroup;
  isSaving = false;

  departments = [1, 2, 3, 4]; // Dummy data for now. Normally we'd fetch via DepartmentService
  roles = [1, 2, 3]; // Employee, Manager, Admin

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employee'] && !changes['employee'].firstChange) {
      this.initForm();
    }
  }

  initForm(): void {
    this.employeeForm = this.fb.group({
      firstName: [this.employee?.firstName || '', [Validators.required]],
      lastName: [this.employee?.lastName || '', [Validators.required]],
      email: [this.employee?.email || '', [Validators.required, Validators.email]],
      phoneNumber: [this.employee?.phoneNumber || ''],
      gender: [this.employee?.gender || 'Male', [Validators.required]],
      dob: [this.employee?.dob || '', [Validators.required]],
      doj: [this.employee?.doj || '', [Validators.required]],
      departmentId: [this.employee?.departmentId || '', [Validators.required]],
      roleId: [this.employee?.roleId || '', [Validators.required]],
      status: [this.employee?.status || 'Active', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) return;

    this.isSaving = true;
    const formValue = this.employeeForm.value;

    if (this.mode === 'add') {
      this.employeeService.createEmployee(formValue).subscribe({
        next: () => {
          this.isSaving = false;
          this.saved.emit(true);
        },
        error: () => {
          this.isSaving = false;
          this.saved.emit(false);
        }
      });
    } else {
      if (!this.employee) return;
      this.employeeService.updateEmployee(this.employee.employeeId, formValue).subscribe({
        next: () => {
          this.isSaving = false;
          this.saved.emit(true);
        },
        error: () => {
          this.isSaving = false;
          this.saved.emit(false);
        }
      });
    }
  }
}
