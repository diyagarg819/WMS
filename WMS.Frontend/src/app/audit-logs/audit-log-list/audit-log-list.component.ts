import { Component, OnInit } from '@angular/core';
import { AuditLogService, AuditLog } from '../../shared/services/audit-log.service';

@Component({
  selector: 'app-audit-log-list',
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss']
})
export class AuditLogListComponent implements OnInit {
  displayedColumns: string[] = ['auditId', 'entityName', 'recordId', 'action', 'createdBy', 'createdOn'];
  dataSource: AuditLog[] = [];
  isLoading = false;

  startDate: Date | null = null;
  endDate: Date | null = null;

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.isLoading = true;
    this.auditLogService.getAuditLogs(this.startDate, this.endDate).subscribe({
      next: (response) => {
        this.dataSource = response.data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    this.loadLogs();
  }

  clearFilter(): void {
    this.startDate = null;
    this.endDate = null;
    this.loadLogs();
  }
}
