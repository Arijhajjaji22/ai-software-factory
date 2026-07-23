export interface Projet {
  id: string;
  ideeMetier: string;
  etapeActuelle: number;
  statutEtapeActuelle: number;
  dateCreation?: string;
  resultatAnalyse: { userStories: any[] } | null;
  resultatArchitecture: any | null;
  resultatDevBackend: any | null;
  resultatDevFrontend: any | null;
  resultatDevOps: any | null;
}
