import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-notification-banner',
  templateUrl: './notification-banner.component.html',
  styleUrls: ['./notification-banner.component.scss'],
  standalone: false
})
export class NotificationBannerComponent {
  @Input() type: 'success' | 'error' | 'warning' | 'info' = 'info';
  @Input() message: string = '';
  @Input() visible: boolean = false;
  @Output() dismissed = new EventEmitter<void>();

  getIcon(): string {
    switch (this.type) {
      case 'success': return 'check_circle';
      case 'error': return 'error';
      case 'warning': return 'warning';
      default: return 'info';
    }
  }

  onDismiss() {
    this.visible = false;
    this.dismissed.emit();
  }
}
