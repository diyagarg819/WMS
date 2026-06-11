import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../shared/services/client.service';
import { ClientRecord } from '../../shared/models/client.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../shared/services/auth.service';
import { Role } from '../../shared/enums/role.enum';

@Component({
  selector: 'app-client-list',
  standalone: false,
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.scss']
})
export class ClientListComponent implements OnInit {
  clients: ClientRecord[] = [];
  displayedColumns: string[] = ['clientId', 'clientName', 'clientLocation', 'status', 'actions'];
  searchTerm = '';
  isLoading = true;

  showFormPanel = false;
  selectedClient: ClientRecord | null = null;

  constructor(
    private clientService: ClientService,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) { }

  get isAdmin(): boolean {
    return this.authService.getRole() === Role.Admin;
  }

  ngOnInit(): void {
    if (!this.isAdmin) {
      this.displayedColumns = ['clientId', 'clientName', 'clientLocation', 'status'];
    }
    this.loadClients();
  }

  loadClients(): void {
    this.isLoading = true;
    this.clientService.getAllClients(this.searchTerm).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.clients = res.data;
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load clients', err);
        this.isLoading = false;
      }
    });
  }

  onSearch(): void {
    this.loadClients();
  }

  openAddForm(): void {
    this.selectedClient = null;
    this.showFormPanel = true;
  }

  openEditForm(client: ClientRecord): void {
    this.selectedClient = client;
    this.showFormPanel = true;
  }

  closeFormPanel(): void {
    this.showFormPanel = false;
    this.selectedClient = null;
  }

  onActionComplete(event: { success: boolean, message: string }): void {
    this.snackBar.open(event.message, 'Close', { duration: 3000 });
    if (event.success) {
      this.closeFormPanel();
      this.loadClients();
    }
  }

  toggleClientStatus(client: ClientRecord): void {
    const updatedStatus = !client.status;
    this.clientService.updateClient(client.clientId, {
      clientName: client.clientName,
      clientAdress: client.clientAdress,
      clientPhoneNumber: client.clientPhoneNumber,
      clientLocation: client.clientLocation,
      status: updatedStatus
    }).subscribe({
      next: (res) => {
        if (res.success) {
          client.status = updatedStatus;
          this.snackBar.open(
            `Client ${updatedStatus ? 'activated' : 'deactivated'} successfully`,
            'Close',
            { duration: 3000 }
          );
        }
      },
      error: () => {
        this.snackBar.open('Failed to update client status', 'Close', { duration: 3000 });
      }
    });
  }
}
