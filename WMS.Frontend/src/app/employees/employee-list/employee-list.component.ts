import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { EmployeeService } from '../../shared/services/employee.service';
import { Employee } from '../../shared/models/employee.model';
import { AuthService } from '../../shared/services/auth.service';
import { Role } from '../../shared/enums/role.enum';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss'],
  standalone: false
})
export class EmployeeListComponent implements OnInit {
  displayedColumns: string[] = ['employeeId', 'name', 'email', 'department', 'role', 'status'];
  employees: Employee[] = [];
  isLoading = true;
  
  searchControl = new FormControl('');

  // Panel state
  isDetailPanelOpen = false;
  isFormPanelOpen = false;
  selectedEmployee: Employee | null = null;
  formMode: 'add' | 'edit' = 'add';

  // Banner State
  bannerVisible = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  constructor(private employeeService: EmployeeService, private authService: AuthService) {}

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  ngOnInit(): void {
    this.loadEmployees();

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.loadEmployees();
    });
  }

  loadEmployees(): void {
    this.isLoading = true;
    this.employeeService.getEmployees(this.searchControl.value || '')
      .subscribe({
        next: (response) => {
          this.employees = response.data;
          this.isLoading = false;
        },
        error: (err) => {
          this.showBanner('error', 'Failed to load employees from the server.');
          this.isLoading = false;
        }
      });
  }

  openDetailPanel(employee: Employee): void {
    this.selectedEmployee = employee;
    this.isDetailPanelOpen = true;
    this.isFormPanelOpen = false;
  }

  openAddForm(): void {
    this.formMode = 'add';
    this.selectedEmployee = null;
    this.isFormPanelOpen = true;
    this.isDetailPanelOpen = false;
  }

  openEditForm(employee: Employee): void {
    this.formMode = 'edit';
    this.isFormPanelOpen = true;
    this.isDetailPanelOpen = false;
    // Fetch full details (list DTO doesn't have departmentId, roleId, gender, dob, doj)
    this.employeeService.getEmployeeById(employee.employeeId).subscribe({
      next: (res) => {
        this.selectedEmployee = res.data;
      },
      error: () => {
        this.selectedEmployee = employee;
      }
    });
  }

  closePanel(): void {
    this.isDetailPanelOpen = false;
    this.isFormPanelOpen = false;
    // Delay setting null to allow the close animation to finish
    setTimeout(() => this.selectedEmployee = null, 300);
  }

  handleFormSubmit(result: boolean | string): void {
    if (result === true) {
      this.showBanner('success', this.formMode === 'add' ? 'Employee created successfully' : 'Employee updated successfully');
      this.closePanel();
      this.loadEmployees();
    } else if (typeof result === 'string') {
      this.showBanner('error', result);
    } else {
      this.showBanner('error', 'An error occurred during save.');
    }
  }

  handleDelete(success: boolean): void {
    if (success) {
      this.showBanner('success', 'Employee deactivated successfully');
      this.closePanel();
      this.loadEmployees();
    }
  }

  showBanner(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.bannerVisible = true;
    setTimeout(() => this.bannerVisible = false, 5000);
  }
}
