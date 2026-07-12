import { Component, signal, Signal } from '@angular/core';

interface Reservation {
  Id: string;
  LivreId: string;
  UsagerId: string;
  DateReservation: string; // ISO string
  Position: number;
  DateExpiration: string;
  Notified: boolean;
}

@Component({
  selector: 'app-reservation-liste',
  standalone: true,
  imports: [],
  template: `
    <h2>Liste des Réservations</h2>
    @if (reservations().length === 0) {
      <p>Aucune réservation.</p>
    } @else {
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Livre Id</th>
            <th>Usager Id</th>
            <th>Date de réservation</th>
            <th>Position</th>
            <th>Date d'expiration</th>
            <th>Notifié</th>
          </tr>
        </thead>
        <tbody>
          @for (r of reservations(); track r.Id) {
            <tr>
              <td>{{ r.Id }}</td>
              <td>{{ r.LivreId }}</td>
              <td>{{ r.UsagerId }}</td>
              <td>{{ r.DateReservation }}</td>
              <td>{{ r.Position }}</td>
              <td>{{ r.DateExpiration }}</td>
              <td>{{ r.Notified }}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class ReservationListeComponent {
  reservations: Signal<Reservation[]> = signal([]);

  constructor() {
    // TODO: charger les réservations depuis une API
    this.reservations.set([
      {
        Id: '11111111-1111-1111-1111-111111111111',
        LivreId: '22222222-2222-2222-2222-222222222222',
        UsagerId: '33333333-3333-3333-3333-333333333333',
        DateReservation: new Date().toISOString(),
        Position: 1,
        DateExpiration: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
        Notified: false
      }
    ]);
  }
}
