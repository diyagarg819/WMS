import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LeaveService } from '../../shared/services/leave.service';

@Component({
  selector: 'app-leave-form-panel',
  templateUrl: './leave-form-panel.component.html',
  styleUrls: ['./leave-form-panel.component.scss'],
  standalone: false
})
export class LeaveFormPanelComponent implements OnInit {
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  leaveForm: FormGroup;
  isSaving = false;
  errorMessage = '';
  
  leaveTypes = ['Sick', 'Casual', 'Earned'];

  constructor(
    private fb: FormBuilder,
    private leaveService: LeaveService
  ) {
    this.leaveForm = this.fb.group({
      leaveType: ['', Validators.required],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      reason: ['']
    }, { validators: this.dateRangeValidator() });
  }

  dateRangeValidator(): import('@angular/forms').ValidatorFn {
    return (control: import('@angular/forms').AbstractControl): import('@angular/forms').ValidationErrors | null => {
      const from = control.get('fromDate')?.value;
      const to = control.get('toDate')?.value;
      
      if (from && to && new Date(to) < new Date(from)) {
        return { dateRangeInvalid: true };
      }
      return null;
    };
  }

  ngOnInit(): void {
    // Component initialized
  }

  onSubmit(): void {
    if (this.leaveForm.invalid) return;

    this.isSaving = true;
    this.errorMessage = '';

    const req = this.leaveForm.value;
    const formatDate = (dateInput: any) => {
      const d = new Date(dateInput);
      d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
      return d.toISOString().split('T')[0];
    };

    if (req.fromDate) {
      req.fromDate = formatDate(req.fromDate);
    }
    if (req.toDate) {
      req.toDate = formatDate(req.toDate);
    }

    this.leaveService.applyLeave(req).subscribe({
      next: (res) => {
        this.actionComplete.emit({ success: true, message: res.message || 'Leave applied successfully.' });
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to apply leave.';
        this.isSaving = false;
      }
    });
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
