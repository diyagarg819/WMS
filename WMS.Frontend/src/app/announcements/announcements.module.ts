import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AnnouncementsRoutingModule } from './announcements-routing.module';
import { AnnouncementListComponent } from './announcement-list/announcement-list.component';
import { AnnouncementFormPanelComponent } from './announcement-form-panel/announcement-form-panel.component';
import { SharedModule } from '../shared/shared.module';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  declarations: [
    AnnouncementListComponent,
    AnnouncementFormPanelComponent
  ],
  imports: [
    CommonModule,
    AnnouncementsRoutingModule,
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
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatTooltipModule
  ]
})
export class AnnouncementsModule { }
