import { Component, OnInit } from '@angular/core';
import { LeaveService } from '../../shared/services/leave.service';
import { AuthService } from '../../shared/services/auth.service';
import { LeaveRecord, LeaveFilter } from '../../shared/models/leave.model';
import { Role } from '../../shared/enums/role.enum';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-leave-list',
  templateUrl: './leave-list.component.html',
  styleUrls: ['./leave-list.component.scss'],
  standalone: false
})
export class LeaveListComponent implements OnInit {
  records: LeaveRecord[] = [];
  isLoading = false;

  filter: LeaveFilter = {
    searchTerm: '',
    status: undefined
  };

  displayedColumns: string[] = ['leaveType', 'dates', 'reason', 'status', 'appliedOn', 'actions'];
  statuses = ['Pending', 'Approved', 'Rejected'];

  showFormPanel = false;
  showActionPanel = false;
  selectedLeave: LeaveRecord | null = null;
  
  // Banner state
  showBanner = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  searchSubject = new Subject<string>();
  
  // Inline confirmation
  deletingLeaveId: number | null = null;

  constructor(
    private leaveService: LeaveService,
    public authService: AuthService
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.filter.searchTerm = term;
      this.loadData();
    });
  }

  ngOnInit(): void {
    if (this.authService.getRole() === Role.Admin) {
      this.displayedColumns = ['employeeName', 'leaveType', 'dates', 'reason', 'status', 'appliedOn', 'actions'];
    }
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const role = this.authService.getRole();
    
    let req;
    if (role === Role.Admin) {
      req = this.leaveService.getAllLeaves(this.filter);
    } else {
      req = this.leaveService.getMyHistory(this.filter);
    }

    req.subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.records = res.data;
        } else {
          this.showNotification('error', res.message || 'Failed to load leaves.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to load leaves.');
        this.isLoading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }
  
  onFilterChange(): void {
    this.loadData();
  }

  openApplyPanel(): void {
    this.selectedLeave = null;
    this.showFormPanel = true;
  }

  openActionPanel(leave: LeaveRecord): void {
    this.selectedLeave = leave;
    this.showActionPanel = true;
  }

  closePanels(): void {
    this.showFormPanel = false;
    this.showActionPanel = false;
    this.selectedLeave = null;
  }

  onActionComplete(event: { success: boolean, message: string }): void {
    this.closePanels();
    if (event.success) {
      this.showNotification('success', event.message);
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  confirmCancel(id: number): void {
    this.deletingLeaveId = id;
  }

  cancelDeletion(): void {
    this.deletingLeaveId = null;
  }

  executeCancel(id: number): void {
    this.leaveService.cancelLeave(id).subscribe({
      next: (res) => {
        this.showNotification('success', res.message || 'Leave cancelled successfully.');
        this.deletingLeaveId = null;
        this.loadData();
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to cancel leave.');
        this.deletingLeaveId = null;
      }
    });
  }

  showNotification(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.showBanner = true;
  }
}
