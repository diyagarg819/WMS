import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-slide-panel',
  templateUrl: './slide-panel.component.html',
  styleUrls: ['./slide-panel.component.scss'],
  standalone: false
})
export class SlidePanelComponent {
  @Input() isOpen = false;
}
