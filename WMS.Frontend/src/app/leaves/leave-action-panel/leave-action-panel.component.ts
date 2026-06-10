import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LeaveService } from '../../shared/services/leave.service';
import { LeaveRecord } from '../../shared/models/leave.model';

@Component({
  selector: 'app-leave-action-panel',
  templateUrl: './leave-action-panel.component.html',
  styleUrls: ['./leave-action-panel.component.scss'],
  standalone: false
})
export class LeaveActionPanelComponent implements OnInit {
  @Input() leave: LeaveRecord | null = null;
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  isSaving = false;
  errorMessage = '';

  constructor(private leaveService: LeaveService) {}

  ngOnInit(): void {
    // Component initialized
  }

  approveLeave(): void {
    if (!this.leave) return;
    this.processAction('Approved');
  }

  rejectLeave(): void {
    if (!this.leave) return;
    this.processAction('Rejected');
  }

  private processAction(status: string): void {
    if (!this.leave) return;
    
    this.isSaving = true;
    this.errorMessage = '';

    this.leaveService.approveOrReject(this.leave.leaveId, { status }).subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: `Leave ${status.toLowerCase()} successfully.` });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || `Failed to ${status.toLowerCase()} leave.`;
        this.isSaving = false;
      }
    });
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
