import { Component, Signal, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Usager {
  id: string;
  nom: string;
  adresse: string;
  courriel: string;
  pieceIdentite: string;
  numeroAbonne: string;
  isActive: boolean;
  dateInscription: string; // ISO string
}

@Component({
  selector: 'app-usager-liste',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (usagers().length === 0) {
      <p>Aucun usager trouvé.</p>
    } @else {
      <table class="table">
        <thead>
          <tr>
            <th>Id</th>
            <th>Nom</th>
            <th>Adresse</th>
            <th>Courriel</th>
            <th>Pièce d'identité</th>
            <th>Numéro d'abonné</th>
            <th>Actif</th>
            <th>Date d'inscription</th>
          </tr>
        </thead>
        <tbody>
          @for (usager of usagers(); track usager.id) {
            <tr>
              <td>{{usager.id}}</td>
              <td>{{usager.nom}}</td>
              <td>{{usager.adresse}}</td>
              <td>{{usager.courriel}}</td>
              <td>{{usager.pieceIdentite}}</td>
              <td>{{usager.numeroAbonne}}</td>
              <td>{{usager.isActive ? 'Oui' : 'Non'}}</td>
              <td>{{usager.dateInscription | date:'short'}}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class UsagerListeComponent {
  usagers: Signal<Usager[]> = signal([]);
}
