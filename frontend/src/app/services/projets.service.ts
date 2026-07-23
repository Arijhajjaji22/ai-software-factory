import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Projet } from '../models/projet.model';

@Injectable({
  providedIn: 'root'
})
export class ProjetsService {
  private base = 'http://localhost:5101/api/projets';
  constructor(private http: HttpClient) { }

  creerProjet(idee: string): Observable<Projet> {
    return this.http.post<Projet>(this.base, { idee });
  }
  getProjet(id: string): Observable<Projet> {
    return this.http.get<Projet>(`${this.base}/${id}`);
  }
  valider(id: string): Observable<Projet> {
    return this.http.post<Projet>(`${this.base}/${id}/valider`, {});
  }
  rejeter(id: string, commentaire: string): Observable<Projet> {
    return this.http.post<Projet>(`${this.base}/${id}/rejeter`, { commentaire });
  }
  telechargerUrl(id: string): string {
  return `${this.base}/${id}/telecharger`;
  }
  listerProjets(): Observable<Projet[]> {
    return this.http.get<Projet[]>(this.base);
  }
}
// (plus de export { Projet }; à la fin)
