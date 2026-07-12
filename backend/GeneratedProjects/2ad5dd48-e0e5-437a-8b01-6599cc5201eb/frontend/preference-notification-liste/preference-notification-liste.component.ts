import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';

interface PreferenceNotification {
  Id: string;
  UsagerId: string;
  CanalEmail: boolean;
  CanalSMS: boolean;
  CanalPush: boolean;
  RappelRetour: boolean;
  CategoriesPref: string;
  Frequence: string;
}

@Component({
  selector: 'app-preference-notification-liste',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  template: `
    <h2>Liste des PreferenceNotification</h2>

    <p @if="preferences().length === 0">Aucun paramètre de notification.</p>

    <table class="table" @if="preferences().length > 0">
      <thead>
        <tr>
          <th>Id</th>
          <th>UsagerId</th>
          <th>Email</th>
          <th>SMS</th>
          <th>Push</th>
          <th>Rappel Retour</th>
          <th>Categories</th>
          <th>Frequence</th>
        </tr>
      </thead>
      <tbody>
        <tr @for="let pref of preferences()">
          <td>{{pref.Id}}</td>
          <td>{{pref.UsagerId}}</td>
          <td>{{pref.CanalEmail}}</td>
          <td>{{pref.CanalSMS}}</td>
          <td>{{pref.CanalPush}}</td>
          <td>{{pref.RappelRetour}}</td>
          <td>{{pref.CategoriesPref}}</td>
          <td>{{pref.Frequence}}</td>
        </tr>
      </tbody>
    </table>
  `
})
export class PreferenceNotificationListeComponent implements OnInit {
  private http = inject(HttpClient);
  preferences = signal<PreferenceNotification[]>([]);

  ngOnInit(): void {
    // Remplacez l'URL par l'endpoint réel de votre API
    this.http.get<PreferenceNotification[]>('/api/preference-notifications')
      .subscribe(data => this.preferences.set(data));
  }
}
