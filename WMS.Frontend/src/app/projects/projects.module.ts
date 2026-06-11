import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { ProjectsRoutingModule } from './projects-routing.module';
import { ProjectListComponent } from './project-list/project-list.component';
import { ProjectFormPanelComponent } from './project-form-panel/project-form-panel.component';
import { ProjectDetailPanelComponent } from './project-detail-panel/project-detail-panel.component';
import { SharedModule } from '../shared/shared.module';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AllocationManagementComponent } from './allocation-management/allocation-management.component';
import { MyProjectsComponent } from './my-projects/my-projects.component';

@NgModule({
  declarations: [
    ProjectListComponent,
    ProjectFormPanelComponent,
    ProjectDetailPanelComponent,
    AllocationManagementComponent,
    MyProjectsComponent
  ],
  imports: [
    CommonModule,
    ProjectsRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule
  ]
})
export class ProjectsModule { }
