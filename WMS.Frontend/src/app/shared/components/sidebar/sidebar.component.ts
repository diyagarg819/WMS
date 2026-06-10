import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Role } from '../../enums/role.enum';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: false
})
export class SidebarComponent {
  Role = Role;

  constructor(public authService: AuthService) {}
}
