import { Component, OnInit } from '@angular/core';
import { AnnouncementService } from '../../shared/services/announcement.service';
import { EmployeeService } from '../../shared/services/employee.service';
import { AuthService } from '../../shared/services/auth.service';
import { AnnouncementRecord } from '../../shared/models/announcement.model';
import { Role } from '../../shared/enums/role.enum';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-announcement-list',
  templateUrl: './announcement-list.component.html',
  styleUrls: ['./announcement-list.component.scss'],
  standalone: false
})
export class AnnouncementListComponent implements OnInit {
  records: AnnouncementRecord[] = [];
  isLoading = false;
  searchTerm = '';
  statusFilter: string = '';

  displayedColumns: string[] = ['title', 'message', 'targetAudience', 'creatorName', 'createdOn', 'status', 'actions'];

  showFormPanel = false;
  editingAnnouncement: AnnouncementRecord | null = null;
  searchSubject = new Subject<string>();

  showBanner = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  // For inline delete confirmation
  deletingId: number | null = null;

  Role = Role;

  // Map of employeeId -> full name for resolving audience
  employeeMap: Map<number, string> = new Map();

  constructor(
    private announcementService: AnnouncementService,
    private employeeService: EmployeeService,
    public authService: AuthService
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.searchTerm = term;
      this.loadData();
    });
  }

  ngOnInit(): void {
    // Employees don't see actions column
    if (this.authService.getRole() === Role.Employee) {
      this.displayedColumns = ['title', 'message', 'targetAudience', 'creatorName', 'createdOn', 'status'];
    }
    this.loadEmployeeMap();
    this.loadData();
  }

  loadEmployeeMap(): void {
    this.employeeService.getEmployees().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          res.data.forEach(emp => {
            this.employeeMap.set(emp.employeeId, `${emp.firstName} ${emp.lastName}`);
          });
        }
      }
    });
  }

  getAudienceDisplay(targetAudience: string): string {
    if (targetAudience === 'All') return 'All Employees';
    const ids = targetAudience.split(',').map(id => parseInt(id.trim(), 10)).filter(id => !isNaN(id));
    const names = ids.map(id => this.employeeMap.get(id) || `#${id}`).slice(0, 3);
    const remaining = ids.length - 3;
    return remaining > 0 ? `${names.join(', ')} +${remaining} more` : names.join(', ');
  }

  get isAdminOrManager(): boolean {
    const role = this.authService.getRole();
    return role === Role.Admin || role === Role.Manager;
  }

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  loadData(): void {
    this.isLoading = true;
    let isActive: boolean | undefined;
    if (this.statusFilter === 'active') isActive = true;
    else if (this.statusFilter === 'inactive') isActive = false;

    this.announcementService.getAll(this.searchTerm || undefined, isActive).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.records = res.data;
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to load announcements.');
        this.isLoading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }

  onStatusFilterChange(): void {
    this.loadData();
  }

  openCreatePanel(): void {
    this.editingAnnouncement = null;
    this.showFormPanel = true;
  }

  openEditPanel(announcement: AnnouncementRecord): void {
    this.editingAnnouncement = announcement;
    this.showFormPanel = true;
  }

  closeFormPanel(): void {
    this.showFormPanel = false;
    this.editingAnnouncement = null;
  }

  onFormComplete(event: { success: boolean, message: string }): void {
    this.closeFormPanel();
    if (event.success) {
      this.showNotification('success', event.message);
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  toggleActive(announcement: AnnouncementRecord): void {
    this.announcementService.toggleActive(announcement.announcementId).subscribe({
      next: (res) => {
        if (res.success) {
          this.showNotification('success', res.message);
          this.loadData();
        }
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to update announcement.');
      }
    });
  }

  confirmDelete(id: number): void {
    this.deletingId = id;
  }

  cancelDelete(): void {
    this.deletingId = null;
  }

  executeDelete(id: number): void {
    this.announcementService.delete(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.showNotification('success', res.message);
          this.deletingId = null;
          this.loadData();
        }
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to delete announcement.');
        this.deletingId = null;
      }
    });
  }

  showNotification(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.showBanner = true;
  }
}
