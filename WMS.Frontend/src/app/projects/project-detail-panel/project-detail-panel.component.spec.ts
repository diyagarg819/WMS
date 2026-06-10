import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectDetailPanelComponent } from './project-detail-panel.component';

describe('ProjectDetailPanelComponent', () => {
  let component: ProjectDetailPanelComponent;
  let fixture: ComponentFixture<ProjectDetailPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProjectDetailPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectDetailPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
