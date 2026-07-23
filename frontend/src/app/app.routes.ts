import { Routes } from '@angular/router';
import { SaisieIdeeComponent } from './features/saisie-idee/saisie-idee.component';
import { SuiviProjetComponent } from './features/suivi-projet/suivi-projet.component';

export const routes: Routes = [
  { path: '', component: SaisieIdeeComponent },
  { path: 'projets/:id', component: SuiviProjetComponent },
];
