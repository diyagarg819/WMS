import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Shared
import { SharedModule } from '../shared/shared.module';

// Components
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmployeeDetailPanelComponent } from './employee-detail-panel/employee-detail-panel.component';
import { EmployeeFormPanelComponent } from './employee-form-panel/employee-form-panel.component';

// Material Imports
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';

const routes: Routes = [
  { path: '', component: EmployeeListComponent }
];

@NgModule({
  declarations: [
    EmployeeListComponent,
    EmployeeDetailPanelComponent,
    EmployeeFormPanelComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes),
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule
  ]
})
export class EmployeesModule { }
