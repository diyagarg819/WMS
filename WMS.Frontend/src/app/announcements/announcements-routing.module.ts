import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AnnouncementListComponent } from './announcement-list/announcement-list.component';

const routes: Routes = [
  { path: '', component: AnnouncementListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AnnouncementsRoutingModule { }
