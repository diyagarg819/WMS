import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectFormPanelComponent } from './project-form-panel.component';

describe('ProjectFormPanelComponent', () => {
  let component: ProjectFormPanelComponent;
  let fixture: ComponentFixture<ProjectFormPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProjectFormPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectFormPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
