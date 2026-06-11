import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee } from '../../shared/models/employee.model';
import { EmployeeService } from '../../shared/services/employee.service';
import { DepartmentService } from '../../shared/services/department.service';
import { Department } from '../../shared/models/department.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

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

  departments: Department[] = [];
  roles: { roleId: number; roleName: string }[] = [];

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadDepartments();
    this.loadRoles();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employee'] && !changes['employee'].firstChange) {
      this.initForm();
    }
  }

  loadDepartments(): void {
    this.departmentService.getAllDepartments().subscribe({
      next: (response) => {
        this.departments = response.data;
      },
      error: () => {
        this.departments = [];
      }
    });
  }

  loadRoles(): void {
    this.http.get<any>(`${environment.apiUrl}/role`).subscribe({
      next: (response) => {
        this.roles = response.data?.data || response.data || [];
      },
      error: () => {
        // Fallback to hardcoded roles if no API exists
        this.roles = [
          { roleId: 1, roleName: 'Admin' },
          { roleId: 2, roleName: 'Manager' },
          { roleId: 3, roleName: 'Employee' }
        ];
      }
    });
  }

  initForm(): void {
    const isAdd = this.mode === 'add';

    this.employeeForm = this.fb.group({
      firstName: [this.employee?.firstName || '', [Validators.required]],
      lastName: [this.employee?.lastName || '', [Validators.required]],
      email: [this.employee?.email || '', [Validators.required, Validators.email, Validators.pattern('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$')]],
      phoneNumber: [this.employee?.phoneNumber || '', [Validators.required, Validators.pattern('^\\d{10}$')]],
      gender: [this.employee?.gender || 'M', [Validators.required]],
      dob: [this.employee?.dob || '', [Validators.required, this.minimumAgeValidator(18)]],
      doj: [this.employee?.doj || '', [Validators.required]],
      departmentId: [this.employee?.departmentId || '', [Validators.required]],
      roleId: [this.employee?.roleId || '', [Validators.required]],
      status: [this.employee?.status || 'Active', [Validators.required]],
      username: [this.employee?.username || 'user' + Math.floor(Math.random() * 1000), [Validators.required, Validators.maxLength(50)]],
      password: [isAdd ? '' : '********', isAdd ? [Validators.required, Validators.minLength(6), Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\\$%\\^&\\*]).{6,}$')] : [Validators.minLength(6)]]
    });
  }

  minimumAgeValidator(minimumAge: number): import('@angular/forms').ValidatorFn {
    return (control: import('@angular/forms').AbstractControl): import('@angular/forms').ValidationErrors | null => {
      if (!control.value) return null;
      
      const dob = new Date(control.value);
      const today = new Date();
      let age = today.getFullYear() - dob.getFullYear();
      const m = today.getMonth() - dob.getMonth();
      
      if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) {
        age--;
      }
      
      return age < minimumAge ? { minimumAge: { requiredAge: minimumAge, actualAge: age } } : null;
    };
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
      
      // If the password is the default mask, don't send it to the backend
      if (formValue.password === '********') {
        formValue.password = '';
      }

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
