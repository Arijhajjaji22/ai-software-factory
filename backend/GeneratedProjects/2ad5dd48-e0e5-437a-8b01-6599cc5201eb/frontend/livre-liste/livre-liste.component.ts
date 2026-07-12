import { Component, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

interface Livre {
  Id: string;
  Titre: string;
  Auteur: string;
  ISBN: string;
  Categorie: string;
  StockTotal: number;
  StockDisponible: number;
}

@Component({
  selector: 'app-livre-liste',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h2>Liste des Livres</h2>
    @if (livres().length === 0) {
      <p>Aucun livre trouvé.</p>
    } @else {
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Titre</th>
            <th>Auteur</th>
            <th>ISBN</th>
            <th>Catégorie</th>
            <th>Stock Total</th>
            <th>Stock Disponible</th>
          </tr>
        </thead>
        <tbody>
          @for (livre of livres(); track livre.Id) {
            <tr>
              <td>{{livre.Id}}</td>
              <td>{{livre.Titre}}</td>
              <td>{{livre.Auteur}}</td>
              <td>{{livre.ISBN}}</td>
              <td>{{livre.Categorie}}</td>
              <td>{{livre.StockTotal}}</td>
              <td>{{livre.StockDisponible}}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class LivreListeComponent {
  private http = inject(HttpClient);
  livres = signal<Livre[]>([]);

  constructor() {
    this.http.get<Livre[]>('/api/livres').subscribe(data => this.livres.set(data));
  }
}
