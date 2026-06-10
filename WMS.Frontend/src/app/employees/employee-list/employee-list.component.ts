import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { EmployeeService } from '../../shared/services/employee.service';
import { Employee } from '../../shared/models/employee.model';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss'],
  standalone: false
})
export class EmployeeListComponent implements OnInit {
  displayedColumns: string[] = ['employeeId', 'name', 'email', 'department', 'role', 'status'];
  employees: Employee[] = [];
  totalItems = 0;
  pageSize = 10;
  currentPage = 1;
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

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.loadEmployees();

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 1;
      this.loadEmployees();
    });
  }

  loadEmployees(): void {
    this.isLoading = true;
    this.employeeService.getEmployees(this.currentPage, this.pageSize, this.searchControl.value || '')
      .subscribe({
        next: (response) => {
          this.employees = response.data.data;
          this.totalItems = response.data.totalCount;
          this.isLoading = false;
        },
        error: (err) => {
          this.showBanner('error', 'Failed to load employees from the server.');
          this.isLoading = false;
        }
      });
  }

  onPageChange(event: any): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadEmployees();
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
    this.selectedEmployee = employee;
    this.isFormPanelOpen = true;
    this.isDetailPanelOpen = false;
  }

  closePanel(): void {
    this.isDetailPanelOpen = false;
    this.isFormPanelOpen = false;
    // Delay setting null to allow the close animation to finish
    setTimeout(() => this.selectedEmployee = null, 300);
  }

  handleFormSubmit(success: boolean): void {
    if (success) {
      this.showBanner('success', this.formMode === 'add' ? 'Employee created successfully' : 'Employee updated successfully');
      this.closePanel();
      this.loadEmployees();
    }
  }

  handleDelete(success: boolean): void {
    if (success) {
      this.showBanner('success', 'Employee deleted successfully');
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
