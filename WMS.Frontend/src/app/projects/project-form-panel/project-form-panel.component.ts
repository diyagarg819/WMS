import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProjectService } from '../../shared/services/project.service';
import { ProjectRecord } from '../../shared/models/project.model';

@Component({
  selector: 'app-project-form-panel',
  templateUrl: './project-form-panel.component.html',
  styleUrls: ['./project-form-panel.component.scss'],
  standalone: false
})
export class ProjectFormPanelComponent implements OnInit {
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() project: ProjectRecord | null = null;
  @Output() closePanel = new EventEmitter<void>();
  @Output() actionComplete = new EventEmitter<{ success: boolean, message: string }>();

  projectForm: FormGroup;
  isSaving = false;
  errorMessage = '';
  
  statuses = ['Active', 'Completed', 'On-Hold'];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService
  ) {
    this.projectForm = this.fb.group({
      projectName: ['', Validators.required],
      clientId: [null],
      startDate: [null],
      endDate: [null],
      status: ['Active', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.mode === 'edit' && this.project) {
      this.projectForm.patchValue({
        projectName: this.project.projectName,
        clientId: this.project.clientId,
        startDate: this.project.startDate,
        endDate: this.project.endDate,
        status: this.project.status
      });
    }
  }

  onSubmit(): void {
    if (this.projectForm.invalid) return;

    this.isSaving = true;
    this.errorMessage = '';

    const req = this.projectForm.value;
    
    // Format dates to ISO string before sending
    if (req.startDate) {
      req.startDate = new Date(req.startDate).toISOString();
    }
    if (req.endDate) {
      req.endDate = new Date(req.endDate).toISOString();
    }

    if (this.mode === 'create') {
      this.projectService.createProject(req).subscribe({
        next: (res) => {
          this.actionComplete.emit({ success: true, message: res.message || 'Project created successfully.' });
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to create project.';
          this.isSaving = false;
        }
      });
    } else if (this.mode === 'edit' && this.project) {
      this.projectService.updateProject(this.project.projectId, req).subscribe({
        next: (res) => {
          this.actionComplete.emit({ success: true, message: res.message || 'Project updated successfully.' });
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to update project.';
          this.isSaving = false;
        }
      });
    }
  }

  onCancel(): void {
    this.closePanel.emit();
  }
}
