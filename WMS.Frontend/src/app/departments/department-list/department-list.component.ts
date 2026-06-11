import { Component, OnInit } from '@angular/core';
import { DepartmentService } from '../../shared/services/department.service';
import { AuthService } from '../../shared/services/auth.service';
import { Department } from '../../shared/models/department.model';
import { Role } from '../../shared/enums/role.enum';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-department-list',
  templateUrl: './department-list.component.html',
  styleUrls: ['./department-list.component.scss'],
  standalone: false
})
export class DepartmentListComponent implements OnInit {
  departments: Department[] = [];
  isLoading = false;
  searchTerm = '';

  displayedColumns: string[] = ['departmentName', 'description', 'createdOn', 'actions'];

  showFormPanel = false;
  selectedDepartment: Department | null = null;
  formMode: 'create' | 'edit' = 'create';
  
  // Banner state
  showBanner = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  searchSubject = new Subject<string>();

  constructor(
    private departmentService: DepartmentService,
    public authService: AuthService
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.searchTerm = term;
      this.loadData();
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.departmentService.getAllDepartments(this.searchTerm).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.departments = res.data;
        } else {
          this.showNotification('error', res.message || 'Failed to load departments.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to load departments.');
        this.isLoading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }

  openCreatePanel(): void {
    this.formMode = 'create';
    this.selectedDepartment = null;
    this.showFormPanel = true;
  }

  openEditPanel(department: Department): void {
    this.formMode = 'edit';
    this.selectedDepartment = department;
    this.showFormPanel = true;
  }

  closePanel(): void {
    this.showFormPanel = false;
    this.selectedDepartment = null;
  }

  onFormComplete(event: { success: boolean, message: string }): void {
    this.closePanel();
    if (event.success) {
      this.showNotification('success', event.message);
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  deleteDepartment(department: Department): void {
    if (!confirm(`Are you sure you want to delete department ${department.departmentName}?`)) return;

    this.departmentService.deleteDepartment(department.departmentId).subscribe({
      next: (res) => {
        this.showNotification('success', res.message || 'Department deleted successfully.');
        this.loadData();
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to delete department.');
      }
    });
  }

  showNotification(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.showBanner = true;
  }

  canManageDepartments(): boolean {
    const role = this.authService.getRole();
    return role === Role.Admin;
  }
}
