import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../shared/services/project.service';
import { ProjectRecord } from '../../shared/models/project.model';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-my-projects',
  templateUrl: './my-projects.component.html',
  styleUrls: ['./my-projects.component.scss'],
  standalone: false
})
export class MyProjectsComponent implements OnInit {
  projects: ProjectRecord[] = [];
  displayedColumns: string[] = ['projectName', 'dates', 'status', 'actions'];
  isLoading = true;

  showDetailPanel = false;
  selectedProject: ProjectRecord | null = null;

  constructor(
    private projectService: ProjectService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadMyProjects();
  }

  loadMyProjects(): void {
    const userId = this.authService.getUserId();

    if (!userId) {
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.projectService.getProjectsByEmployee(userId).subscribe({
      next: (res) => {
        if (res.data) {
          this.projects = res.data;
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load my projects', err);
        this.isLoading = false;
      }
    });
  }

  openDetailPanel(project: ProjectRecord): void {
    this.selectedProject = { ...project };
    this.showDetailPanel = true;
  }

  closePanels(): void {
    this.showDetailPanel = false;
    this.selectedProject = null;
  }
}
