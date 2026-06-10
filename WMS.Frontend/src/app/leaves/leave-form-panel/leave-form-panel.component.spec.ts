import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveFormPanelComponent } from './leave-form-panel.component';

describe('LeaveFormPanelComponent', () => {
  let component: LeaveFormPanelComponent;
  let fixture: ComponentFixture<LeaveFormPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveFormPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeaveFormPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
