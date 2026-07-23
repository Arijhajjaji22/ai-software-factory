import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ProjetsService } from '../../services/projets.service';
import { Projet } from '../../models/projet.model';

@Component({
  selector: 'app-suivi-projet',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './suivi-projet.component.html',
  styleUrl: './suivi-projet.component.scss'
})
export class SuiviProjetComponent implements OnInit {
  projet: Projet | null = null;
  chargement = true;
  erreur: string | null = null;

  afficherFormRejet = false;
  commentaireRejet = '';

  readonly etapes = ['Analyse', 'Architecture', 'Backend', 'Frontend', 'DevOps', 'Terminé'];
  readonly labelsStatut: Record<number, string> = {
    0: 'En attente de validation',
    1: 'Validé',
  };

  private id!: string;

  constructor(
    private route: ActivatedRoute,
    public projetsService: ProjetsService   // public : le template peut l'utiliser directement
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.charger();
  }

  get statutLabel(): string {
    return this.labelsStatut[this.projet?.statutEtapeActuelle ?? 0] ?? 'Inconnu';
  }

  charger(): void {
    this.chargement = true;
    this.erreur = null;
    this.projetsService.getProjet(this.id).subscribe({
      next: (p) => { this.projet = p; this.chargement = false; },
      error: (err) => { this.erreur = 'Impossible de charger le projet.'; this.chargement = false; console.error(err); }
    });
  }

  valider(): void {
    this.chargement = true;
    this.erreur = null;
    this.projetsService.valider(this.id).subscribe({
      next: (p) => { this.projet = p; this.chargement = false; },
      error: (err) => { this.erreur = 'Erreur lors de la validation.'; this.chargement = false; console.error(err); }
    });
  }

  rejeter(): void {
    this.chargement = true;
    this.erreur = null;
    this.projetsService.rejeter(this.id, this.commentaireRejet).subscribe({
      next: (p) => {
        this.projet = p;
        this.chargement = false;
        this.afficherFormRejet = false;
        this.commentaireRejet = '';
      },
      error: (err) => { this.erreur = 'Erreur lors du rejet.'; this.chargement = false; console.error(err); }
    });
  }
}
