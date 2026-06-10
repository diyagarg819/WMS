import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentFormPanelComponent } from './department-form-panel.component';

describe('DepartmentFormPanelComponent', () => {
  let component: DepartmentFormPanelComponent;
  let fixture: ComponentFixture<DepartmentFormPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DepartmentFormPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DepartmentFormPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
