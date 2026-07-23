import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ProjetsService } from '../../services/projets.service';
import { Projet } from '../../models/projet.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  projets: Projet[] = [];

  readonly etapes = ['Analyse', 'Architecture', 'Backend', 'Frontend', 'DevOps', 'Terminé'];

  constructor(private projetsService: ProjetsService, private router: Router) { }

  ngOnInit(): void {
    this.charger();
  }

  charger(): void {
    this.projetsService.listerProjets().subscribe({
      next: (p) => this.projets = p,
      error: (err) => console.error(err)
    });
  }

  nouveauProjet(): void {
    this.router.navigate(['/']);
  }

  etapeLabel(etapeActuelle: number): string {
    return this.etapes[etapeActuelle] ?? 'Inconnu';
  }
}
