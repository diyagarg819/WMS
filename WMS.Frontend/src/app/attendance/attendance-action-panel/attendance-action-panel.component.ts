import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AttendanceService } from '../../shared/services/attendance.service';
import { AttendanceRecord } from '../../shared/models/attendance.model';

@Component({
  selector: 'app-attendance-action-panel',
  templateUrl: './attendance-action-panel.component.html',
  styleUrls: ['./attendance-action-panel.component.scss'],
  standalone: false
})
export class AttendanceActionPanelComponent implements OnInit {
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  todayStatus: AttendanceRecord | null = null;
  isLoading = true;
  isSaving = false;
  errorMessage = '';

  checkInForm: FormGroup;
  
  workModes = ['WFO', 'WFH', 'Hybrid'];

  constructor(
    private fb: FormBuilder,
    private attendanceService: AttendanceService
  ) {
    this.checkInForm = this.fb.group({
      workMode: ['WFO', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadTodayStatus();
  }

  loadTodayStatus(): void {
    this.isLoading = true;
    this.attendanceService.getTodayStatus().subscribe({
      next: (res) => {
        if (res.success) {
          this.todayStatus = res.data;
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to load today\'s status.';
        this.isLoading = false;
      }
    });
  }

  onCheckIn(): void {
    if (this.checkInForm.invalid) return;

    this.isSaving = true;
    this.attendanceService.checkIn(this.checkInForm.value).subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: res.message || 'Checked in successfully.' });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to check in.';
        this.isSaving = false;
      }
    });
  }

  onCheckOut(): void {
    this.isSaving = true;
    this.attendanceService.checkOut().subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: res.message || 'Checked out successfully.' });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to check out.';
        this.isSaving = false;
      }
    });
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
