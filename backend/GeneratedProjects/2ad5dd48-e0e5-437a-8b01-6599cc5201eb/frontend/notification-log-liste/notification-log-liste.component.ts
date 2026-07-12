import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface NotificationLog {
  Id: string;
  UsagerId: string;
  Type: string;
  DateEnvoi: string;
  Details: string;
}

@Component({
  selector: 'app-notification-log-liste',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>Historique des notifications</h2>
    @if (logs().length === 0) {
      <p>Aucun enregistrement.</p>
    } @else {
      <table class=\"table\">
        <thead>
          <tr>
            <th>Id</th>
            <th>Usager Id</th>
            <th>Type</th>
            <th>Date d'envoi</th>
            <th>Détails</th>
          </tr>
        </thead>
        <tbody>
          @for (log of logs(); track log.Id) {
            <tr>
              <td>{{log.Id}}</td>
              <td>{{log.UsagerId}}</td>
              <td>{{log.Type}}</td>
              <td>{{log.DateEnvoi | date:'short'}}</td>
              <td>{{log.Details}}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class NotificationLogListeComponent implements OnInit {
  logs = signal<NotificationLog[]>([]);

  ngOnInit(): void {
    this.loadLogs();
  }

  private async loadLogs(): Promise<void> {
    try {
      const response = await fetch('/api/notificationlogs');
      if (!response.ok) {
        throw new Error('Erreur lors du chargement des logs');
      }
      const data: NotificationLog[] = await response.json();
      this.logs.set(data);
    } catch (error) {
      console.error(error);
    }
  }
}
