import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

interface Exemplaire {
  Id: string;
  LivreId: string;
  CodeBarre: string;
  Disponible: boolean;
}

@Component({
  selector: 'app-exemplaire-liste',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  template: `
    <h2>Liste des Exemplaires</h2>
    @if (exemplaires().length === 0) {
      <p>Aucun exemplaire trouvé.</p>
    } @else {
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Livre Id</th>
            <th>Code Barre</th>
            <th>Disponible</th>
          </tr>
        </thead>
        <tbody>
          @for (ex of exemplaires(); track ex.Id) {
            <tr>
              <td>{{ ex.Id }}</td>
              <td>{{ ex.LivreId }}</td>
              <td>{{ ex.CodeBarre }}</td>
              <td>{{ ex.Disponible ? 'Oui' : 'Non' }}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `
})
export class ExemplaireListeComponent implements OnInit {
  private http = inject(HttpClient);
  exemplaires: Signal<Exemplaire[]> = signal([]);

  ngOnInit(): void {
    this.http.get<Exemplaire[]>('/api/exemplaires')
      .subscribe(data => this.exemplaires.set(data));
  }
}
