import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuditLogsRoutingModule } from './audit-logs-routing.module';
import { AuditLogListComponent } from './audit-log-list/audit-log-list.component';
import { SharedModule } from '../shared/shared.module';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    AuditLogListComponent
  ],
  imports: [
    CommonModule,
    AuditLogsRoutingModule,
    SharedModule,
    MatTableModule,
    MatPaginatorModule,
    MatIconModule,
    MatProgressSpinnerModule
  ]
})
export class AuditLogsModule { }
