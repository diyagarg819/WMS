import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceActionPanelComponent } from './attendance-action-panel.component';

describe('AttendanceActionPanelComponent', () => {
  let component: AttendanceActionPanelComponent;
  let fixture: ComponentFixture<AttendanceActionPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AttendanceActionPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AttendanceActionPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
