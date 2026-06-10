import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DepartmentService } from '../../shared/services/department.service';
import { Department } from '../../shared/models/department.model';

@Component({
  selector: 'app-department-form-panel',
  templateUrl: './department-form-panel.component.html',
  styleUrls: ['./department-form-panel.component.scss'],
  standalone: false
})
export class DepartmentFormPanelComponent implements OnInit {
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() department: Department | null = null;
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  departmentForm: FormGroup;
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService
  ) {
    this.departmentForm = this.fb.group({
      departmentName: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    if (this.mode === 'edit' && this.department) {
      this.departmentForm.patchValue({
        departmentName: this.department.departmentName,
        description: this.department.description
      });
    }
  }

  onSubmit(): void {
    if (this.departmentForm.invalid) return;

    this.isSaving = true;
    this.errorMessage = '';

    const req = this.departmentForm.value;

    if (this.mode === 'create') {
      this.departmentService.createDepartment(req).subscribe({
        next: (res) => {
          this.actionComplete.emit({ success: true, message: res.message || 'Department created successfully.' });
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to create department.';
          this.isSaving = false;
        }
      });
    } else if (this.mode === 'edit' && this.department) {
      this.departmentService.updateDepartment(this.department.departmentId, req).subscribe({
        next: (res) => {
          this.actionComplete.emit({ success: true, message: res.message || 'Department updated successfully.' });
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to update department.';
          this.isSaving = false;
        }
      });
    }
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
