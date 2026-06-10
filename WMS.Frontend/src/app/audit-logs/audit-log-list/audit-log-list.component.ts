import { Component, OnInit, ViewChild } from '@angular/core';
import { AuditLogService, AuditLog } from '../../shared/services/audit-log.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-audit-log-list',
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss']
})
export class AuditLogListComponent implements OnInit {
  displayedColumns: string[] = ['auditId', 'entityName', 'recordId', 'action', 'createdBy', 'createdOn'];
  dataSource: AuditLog[] = [];
  
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.isLoading = true;
    this.auditLogService.getAuditLogs(this.pageIndex + 1, this.pageSize).subscribe({
      next: (response) => {
        this.dataSource = response.data.data;
        this.totalCount = response.data.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadLogs();
  }
}
