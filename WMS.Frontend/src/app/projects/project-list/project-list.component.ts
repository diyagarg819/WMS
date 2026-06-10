import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ProjectService } from '../../shared/services/project.service';
import { AuthService } from '../../shared/services/auth.service';
import { ProjectRecord } from '../../shared/models/project.model';
import { Role } from '../../shared/enums/role.enum';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.scss'],
  standalone: false
})
export class ProjectListComponent implements OnInit {
  projects: ProjectRecord[] = [];
  totalRecords = 0;
  isLoading = false;

  pageNumber = 1;
  pageSize = 10;
  searchTerm = '';

  displayedColumns: string[] = ['projectName', 'dates', 'status', 'actions'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  
  showFormPanel = false;
  showDetailPanel = false;
  
  selectedProject: ProjectRecord | null = null;
  formMode: 'create' | 'edit' = 'create';
  
  // Banner state
  showBanner = false;
  bannerType: 'success' | 'error' | 'warning' | 'info' = 'info';
  bannerMessage = '';

  searchSubject = new Subject<string>();

  constructor(
    private projectService: ProjectService,
    public authService: AuthService
  ) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.searchTerm = term;
      this.pageNumber = 1;
      this.loadData();
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.projectService.getAllProjects(this.pageNumber, this.pageSize, this.searchTerm).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.projects = res.data.data;
          this.totalRecords = res.data.totalCount;
        } else {
          this.showNotification('error', res.message || 'Failed to load projects.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.showNotification('error', err.error?.message || 'Failed to load projects.');
        this.isLoading = false;
      }
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchSubject.next(target.value);
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  openCreatePanel(): void {
    this.formMode = 'create';
    this.selectedProject = null;
    this.showFormPanel = true;
  }

  openEditPanel(project: ProjectRecord): void {
    this.formMode = 'edit';
    this.selectedProject = project;
    this.showFormPanel = true;
  }

  openDetailPanel(project: ProjectRecord): void {
    this.selectedProject = project;
    this.showDetailPanel = true;
  }

  closePanels(): void {
    this.showFormPanel = false;
    this.showDetailPanel = false;
    this.selectedProject = null;
  }

  onFormComplete(event: { success: boolean, message: string }): void {
    this.closePanels();
    if (event.success) {
      this.showNotification('success', event.message);
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  onDetailActionComplete(event: { success: boolean, message: string }): void {
    if (event.success) {
      this.showNotification('success', event.message);
      // Reload the project details and the list
      if (this.selectedProject) {
        this.projectService.getProjectById(this.selectedProject.projectId).subscribe(res => {
          if (res.success && res.data) {
            this.selectedProject = res.data;
          }
        });
      }
      this.loadData();
    } else {
      this.showNotification('error', event.message);
    }
  }

  showNotification(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.bannerType = type;
    this.bannerMessage = message;
    this.showBanner = true;
  }

  canManageProjects(): boolean {
    const role = this.authService.getRole();
    return role === Role.Admin || role === Role.Manager;
  }
}
