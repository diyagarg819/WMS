import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClientRecord } from '../../shared/models/client.model';
import { ClientService } from '../../shared/services/client.service';

@Component({
  selector: 'app-client-form-panel',
  standalone: false,
  templateUrl: './client-form-panel.component.html',
  styleUrls: ['./client-form-panel.component.scss']
})
export class ClientFormPanelComponent implements OnInit {
  @Input() mode: 'add' | 'edit' = 'add';
  @Input() client: ClientRecord | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  clientForm: FormGroup;
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService
  ) {
    this.clientForm = this.fb.group({
      clientName: ['', [Validators.required, Validators.maxLength(100)]],
      clientAdress: ['', Validators.required],
      clientPhoneNumber: [null, [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      clientLocation: ['', [Validators.required, Validators.maxLength(20)]],
      status: [true, Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.mode === 'edit' && this.client) {
      this.clientForm.patchValue({
        clientName: this.client.clientName,
        clientAdress: this.client.clientAdress,
        clientPhoneNumber: this.client.clientPhoneNumber,
        clientLocation: this.client.clientLocation,
        status: this.client.status
      });
    }
  }

  onSubmit(): void {
    if (this.clientForm.invalid) return;

    this.isSaving = true;
    this.errorMessage = '';

    const req = this.clientForm.value;

    if (this.mode === 'add') {
      this.clientService.createClient(req).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res.success) {
            this.actionComplete.emit({ success: true, message: 'Client created successfully' });
          } else {
            this.errorMessage = res.message || 'Error creating client';
          }
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Server error occurred';
        }
      });
    } else if (this.mode === 'edit' && this.client) {
      this.clientService.updateClient(this.client.clientId, req).subscribe({
        next: (res) => {
          this.isSaving = false;
          if (res.success) {
            this.actionComplete.emit({ success: true, message: 'Client updated successfully' });
          } else {
            this.errorMessage = res.message || 'Error updating client';
          }
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Server error occurred';
        }
      });
    }
  }
}
