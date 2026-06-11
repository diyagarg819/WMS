import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ThemeService } from '../../services/theme.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  standalone: false
})
export class NavbarComponent {
  @Output() toggleSidenav = new EventEmitter<void>();
  isDarkMode$: any;

  constructor(
    private authService: AuthService, 
    private themeService: ThemeService,
    private router: Router
  ) {
    this.isDarkMode$ = this.themeService.isDarkMode$;
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  logout(): void {
    this.authService.logout();
  }
}
