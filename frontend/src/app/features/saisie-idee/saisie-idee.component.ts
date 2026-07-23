import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjetsService } from '../../services/projets.service';

@Component({
  selector: 'app-saisie-idee',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './saisie-idee.component.html',
  styleUrl: './saisie-idee.component.scss'
})
export class SaisieIdeeComponent {
  idee = '';
  enCours = false;
  erreur: string | null = null;

  constructor(private projetsService: ProjetsService, private router: Router) { }

  onSubmit(): void {
    if (!this.idee.trim()) return;

    this.enCours = true;
    this.erreur = null;

    this.projetsService.creerProjet(this.idee).subscribe({
      next: (projet) => {
        this.enCours = false;
        this.router.navigate(['/projets', projet.id]);
      },
      error: (err) => {
        this.erreur = "Erreur lors de la création du projet. Vérifie que l'API tourne bien.";
        this.enCours = false;
        console.error(err);
      }
    });
  }
}
