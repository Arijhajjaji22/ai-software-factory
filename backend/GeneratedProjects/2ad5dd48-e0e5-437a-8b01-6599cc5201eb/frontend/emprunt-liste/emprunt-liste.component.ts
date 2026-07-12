import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Emprunt {
  Id: string;
  ExemplaireId: string;
  UsagerId: string;
  DateDebut: Date;
  DateFinPrevue: Date;
  DateRetour: Date | null;
  FraisRetard: number;
}

@Component({
  selector: 'app-emprunt-liste',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>Liste des emprunts</h2>
    @if (emprunts().length === 0) {
      <p>Aucun emprunt trouvé.</p>
    } @else {
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Exemplaire Id</th>
            <th>Usager Id</th>
            <th>Date début</th>
            <th>Date fin prévue</th>
            <th>Date retour</th>
            <th>Frais retard</th>
          </tr>
        </thead>
        <tbody>
          @for (emprunt of emprunts(); track emprunt.Id) {
            <tr>
              <td>{{ emprunt.Id }}</td>
              <td>{{ emprunt.ExemplaireId }}</td>
              <td>{{ emprunt.UsagerId }}</td>
              <td>{{ emprunt.DateDebut | date:'short' }}</td>
              <td>{{ emprunt.DateFinPrevue | date:'short' }}</td>
              <td>
                @if (emprunt.DateRetour) {
                  {{ emprunt.DateRetour | date:'short' }}
                } @else {
                  -
                }
              </td>
              <td>{{ emprunt.FraisRetard | number:'1.2-2' }}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class EmpruntListeComponent {
  emprunts = signal<Emprunt[]>([]);
}
