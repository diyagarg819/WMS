import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { AttendanceService } from '../../shared/services/attendance.service';
import { AuthService } from '../../shared/services/auth.service';
import { AttendanceRecord, AttendanceFilter } from '../../shared/models/attendance.model';
import { Role } from '../../shared/enums/role.enum';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-attendance-list',
  templateUrl: './attendance-list.component.html',
  styleUrls: ['./attendance-list.component.scss'],
  standalone: false
})
export class AttendanceListComponent implements OnInit {
  records: AttendanceRecord[] = [];
  totalRecords = 0;
  isLoading = false;

  filter: AttendanceFilter = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    fromDate: undefined,
    toDate: undefined
  };

  displayedColumns: string[] = ['attendanceDate', 'employeeName', 'checkIn', 'checkOut', 'totalHours', 'workMode'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  
  showActionPanel = false;
  searchSubject = new Subject<string>();
  
  // Banner state
  showBanner = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  constructor(
    private attendanceService: AttendanceService,
    public authService: AuthService
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.filter.searchTerm = term;
      this.filter.pageNumber = 1;
      this.loadData();
    });
  }

  ngOnInit(): void {
    if (this.authService.getRole() === Role.Employee) {
      this.displayedColumns = ['attendanceDate', 'checkIn', 'checkOut', 'totalHours', 'workMode'];
    }
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const req = this.authService.getRole() === Role.Employee 
      ? this.attendanceService.getMyHistory(this.filter)
      : this.attendanceService.getAllAttendance(this.filter);

    req.subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.records = res.data.data;
          this.totalRecords = res.data.totalCount;
        } else {
          this.showNotification('error', res.message || 'Failed to load attendance records.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to load attendance records.');
        this.isLoading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }

  onPageChange(event: PageEvent): void {
    this.filter.pageNumber = event.pageIndex + 1;
    this.filter.pageSize = event.pageSize;
    this.loadData();
  }
  
  onFilterChange(): void {
    this.filter.pageNumber = 1;
    this.loadData();
  }

  openActionPanel(): void {
    this.showActionPanel = true;
  }

  closeActionPanel(): void {
    this.showActionPanel = false;
  }

  onActionComplete(event: { success: boolean, message: string }): void {
    this.closeActionPanel();
    if (event.success) {
      this.showNotification('success', event.message);
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  showNotification(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.showBanner = true;
  }
}
