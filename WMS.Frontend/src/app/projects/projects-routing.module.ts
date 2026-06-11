import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProjectListComponent } from './project-list/project-list.component';
import { AuthGuard } from '../shared/guards/auth.guard';
import { AllocationManagementComponent } from './allocation-management/allocation-management.component';
import { MyProjectsComponent } from './my-projects/my-projects.component';

const routes: Routes = [
  { 
    path: '', 
    component: ProjectListComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'allocations', 
    component: AllocationManagementComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'my-projects', 
    component: MyProjectsComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProjectsRoutingModule { }
