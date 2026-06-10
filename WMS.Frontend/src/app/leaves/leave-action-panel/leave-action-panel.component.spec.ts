import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveActionPanelComponent } from './leave-action-panel.component';

describe('LeaveActionPanelComponent', () => {
  let component: LeaveActionPanelComponent;
  let fixture: ComponentFixture<LeaveActionPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveActionPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeaveActionPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
