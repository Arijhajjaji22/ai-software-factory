# 🏭 AI Software Factory

> Générateur d'applications full-stack assisté par IA — une plateforme multi-agents qui simule un cycle complet de développement logiciel, de l'idée métier jusqu'au code déployable.

[![Backend](https://img.shields.io/badge/backend-.NET%209-512BD4)](https://dotnet.microsoft.com/)
[![Frontend](https://img.shields.io/badge/frontend-Angular%2019-DD0031)](https://angular.io/)
[![Database](https://img.shields.io/badge/database-Azure%20SQL-CC2927)](https://azure.microsoft.com/products/azure-sql/database)
[![Deployment](https://img.shields.io/badge/deployed-Azure%20Container%20Apps-0078D4)](https://azure.microsoft.com/products/container-apps)

---

## 📖 Table des matières

- [Aperçu](#-aperçu)
- [Démo](#-démo)
- [Architecture](#-architecture)
- [Le pipeline d'agents IA](#-le-pipeline-dagents-ia)
- [Stack technique](#-stack-technique)
- [Installation locale](#-installation-locale)
- [Déploiement](#-déploiement)
- [Structure du projet](#-structure-du-projet)
- [Défis techniques rencontrés](#-défis-techniques-rencontrés)
- [Roadmap / améliorations possibles](#-roadmap--améliorations-possibles)

---

## 🎯 Aperçu

**AI Software Factory** transforme une idée métier exprimée en langage naturel en une application full-stack générée automatiquement — backend .NET, frontend Angular, Dockerfiles et pipeline CI/CD — via une chaîne d'agents IA spécialisés, avec **validation humaine à chaque étape**.

Plutôt que de générer toute l'application d'un coup sans contrôle, le pipeline s'arrête après chaque agent et attend une action explicite de l'utilisateur : **valider** pour continuer, ou **rejeter** avec un commentaire pour régénérer. Ce pattern *human-in-the-loop* garde le contrôle qualité au centre du processus.

### Exemple concret

```
Idée métier : "Une application pour gérer une salle de sport : les adhérents 
peuvent s'inscrire à des cours collectifs, réserver des créneaux, et le 
personnel peut suivre les paiements d'abonnement."

        ↓ Agent Analyste
User stories structurées (rôle, action, bénéfice, critères d'acceptation)

        ↓ Agent Architecte
Modèle de données (entités, attributs, relations)

        ↓ Agent Dev Backend
Code C# généré (modèles EF Core, contrôleurs REST, DbContext)

        ↓ Agent Dev Frontend
Composants Angular générés (listes connectées au backend)

        ↓ Agent DevOps
Dockerfiles + pipeline GitHub Actions

        ↓
Application téléchargeable en .zip, prête à builder
```

---

## 🖥 Démo

L'application est déployée et accessible publiquement :

- **Frontend** : `https://aisf-frontend-app.<...>.azurecontainerapps.io`
- **Backend (API)** : `https://aisf-api-app.<...>.azurecontainerapps.io`

> ⚠️ Démo publique sans authentification — à des fins de portfolio uniquement. Voir la section [Défis techniques](#-défis-techniques-rencontrés) pour les limites connues.

---

## 🏗 Architecture

```
┌─────────────────┐         ┌──────────────────────┐         ┌─────────────────┐
│                  │  HTTP   │                       │  SQL    │                 │
│  Angular 19      │────────▶│  ASP.NET Core 9       │────────▶│  Azure SQL      │
│  (Frontend)      │◀────────│  Minimal API          │◀────────│  Database       │
│                  │         │  (Orchestrateur)      │         │  (Serverless)   │
└─────────────────┘         └───────────┬───────────┘         └─────────────────┘
                                         │
                                         │ Semantic Kernel
                                         ▼
                              ┌──────────────────────┐
                              │   Groq API            │
                              │   (openai/gpt-oss-120b)│
                              └──────────────────────┘
```

**Composants principaux :**

| Composant | Rôle |
|---|---|
| **API Backend** (ASP.NET Core Minimal API) | Orchestre le pipeline, expose les endpoints REST, gère l'état du workflow |
| **Semantic Kernel** | Abstraction pour l'appel au LLM (ChatHistory, prompts système, function calling potentiel) |
| **Groq** | Fournisseur d'inférence LLM (API compatible OpenAI), modèle `openai/gpt-oss-120b` |
| **Entity Framework Core + Azure SQL** | Persistance de l'état des projets et de leur historique |
| **Frontend Angular** | Interface de pilotage : saisie idée, suivi visuel du pipeline, validation/rejet, téléchargement |
| **Azure Container Apps** | Hébergement conteneurisé du backend et du frontend |
| **Azure Container Registry** | Stockage des images Docker |
| **GitHub Actions** | CI/CD — build, push, déploiement automatique à chaque push |

---

## 🤖 Le pipeline d'agents IA

Chaque agent est un appel au LLM avec un **prompt système dédié ("persona")**, recevant en entrée le résultat structuré de l'agent précédent, et produisant une **sortie JSON strictement formatée**.

| # | Agent | Entrée | Sortie |
|---|---|---|---|
| 1 | **Analyste** | Idée métier (texte libre) | User stories (rôle, action, bénéfice, critères d'acceptation) |
| 2 | **Architecte** | User stories validées | Entités, attributs, relations |
| 3 | **Dev Backend** | Architecture validée | Modèles C#, contrôleurs REST, DbContext (EF Core) |
| 4 | **Dev Frontend** | Architecture validée | Composants Angular (listes connectées à l'API) |
| 5 | **DevOps** | Architecture validée | Dockerfiles (backend + frontend), pipeline GitHub Actions |

Le pipeline est modélisé comme une **machine à états** : `Analyse → Architecture → DevBackend → DevFrontend → DevOps → Terminé`, avec un statut par étape (`EnAttenteDeValidation` / `Valide`).

### Workflow de validation humaine

```
POST /api/projets              → crée le projet, déclenche l'agent Analyste
POST /api/projets/{id}/valider → valide l'étape actuelle, déclenche l'agent suivant
POST /api/projets/{id}/rejeter → régénère l'étape actuelle avec un feedback utilisateur
GET  /api/projets/{id}         → consulte l'état complet d'un projet
GET  /api/projets              → liste tous les projets
GET  /api/projets/{id}/telecharger → télécharge le projet généré en .zip
```

---

## 🛠 Stack technique

**Backend**
- .NET 9 / ASP.NET Core Minimal API
- Microsoft Semantic Kernel (connecteur OpenAI-compatible)
- Entity Framework Core 9
- Azure SQL Database (tier Serverless, auto-pause)

**Frontend**
- Angular 19 (standalone components, signals où pertinent)
- TypeScript

**IA**
- Groq API (inférence LLM à haute vitesse, `openai/gpt-oss-120b`)

**Infrastructure & DevOps**
- Docker (multi-stage builds)
- Azure Container Apps + Azure Container Registry
- GitHub Actions (CI/CD, authentification OIDC via Managed Identity)

---

## 💻 Installation locale

### Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) + Angular CLI (`npm install -g @angular/cli`)
- SQL Server LocalDB (inclus avec Visual Studio) ou Docker
- Une clé API [Groq](https://console.groq.com/) (gratuite)

### Backend

```bash
cd backend/AiSoftwareFactory.Api

# Configurer la clé Groq (via user-secrets, jamais en dur dans le code)
dotnet user-secrets set "Groq:ApiKey" "votre-clé-api"

# Appliquer les migrations EF Core
dotnet ef database update --project ../AiSoftwareFactory.Agents --startup-project .

# Lancer l'API
dotnet run
```
L'API démarre sur `http://localhost:5101`.

### Frontend

```bash
cd frontend
npm install
ng serve
```
L'application démarre sur `http://localhost:4200`.

---

## ☁️ Déploiement

L'application est déployée sur **Azure Container Apps**, avec une base de données **Azure SQL** et un registre **Azure Container Registry**.

### Infrastructure Azure

```bash
# Groupe de ressources
az group create --name rg-aisf-france --location <region>

# Base de données (serverless, auto-pause après 60 min d'inactivité)
az sql db create --resource-group rg-aisf-france --server <server> \
  --name AiSoftwareFactoryDb --edition GeneralPurpose --family Gen5 \
  --capacity 1 --compute-model Serverless --auto-pause-delay 60

# Container Registry
az acr create --resource-group rg-aisf-france --name <acr-name> --sku Basic

# Container Apps Environment + Apps
az containerapp env create --name aisf-env --resource-group rg-aisf-france
az containerapp create --name aisf-api-app --resource-group rg-aisf-france \
  --environment aisf-env --image <acr>.azurecr.io/aisf-api:latest
```

### CI/CD — GitHub Actions

Deux workflows indépendants (`deploy-backend.yml`, `deploy-frontend.yml`), déclenchés à chaque push sur `main` touchant le dossier concerné :

1. Build de l'image Docker
2. Push vers Azure Container Registry
3. Mise à jour du Container App

L'authentification vers Azure utilise une **Managed Identity** avec **Federated Credentials (OIDC)** — aucun secret longue durée stocké côté GitHub, uniquement des jetons temporaires échangés à chaque run.

```yaml
permissions:
  id-token: write
  contents: read

- uses: azure/login@v2
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

---

## 📁 Structure du projet

```
ai-software-factory/
├── backend/
│   ├── AiSoftwareFactory.Api/        # API Minimal, endpoints, Program.cs
│   ├── AiSoftwareFactory.Agents/     # Personas, modèles, DbContext, ProjetStore
│   ├── GeneratedProjects/            # Sortie des projets générés (gitignored)
│   └── Dockerfile
├── frontend/
│   ├── src/app/
│   │   ├── features/
│   │   │   ├── saisie-idee/          # Écran de création de projet
│   │   │   └── suivi-projet/         # Écran de suivi + validation
│   │   ├── shared/sidebar/           # Liste des projets existants
│   │   ├── services/                 # ProjetsService (appels HTTP)
│   │   └── models/                   # Interfaces TypeScript
│   └── Dockerfile
└── .github/workflows/
    ├── deploy-backend.yml
    └── deploy-frontend.yml
```

---

## 🐛 Défis techniques rencontrés

Quelques problèmes concrets rencontrés et résolus pendant le développement — utile pour comprendre les choix d'architecture :

- **Troncature JSON** : avec un grand nombre d'entités, les réponses du LLM dépassaient la limite de tokens configurée, cassant le parsing. → Augmentation de `MaxTokens`, gestion d'erreur *par entité* pour isoler les échecs sans faire planter tout le pipeline.
- **Rate-limiting Groq (HTTP 429)** : quota journalier du tier gratuit. → Retry avec backoff exponentiel.
- **Cohérence d'état** : un crash en plein milieu d'une transition d'étape pouvait laisser un projet bloqué (ni validé, ni en attente).
- **Restrictions de région Azure for Students** : certaines régions Azure sont indisponibles selon le compte, sans liste claire — résolu par test itératif et vérification via `az policy`.
- **Authentification OIDC GitHub → Azure** : format de `subject claim` non standard observé sur ce compte GitHub, nécessitant une Federated Credential adaptée en plus de la credential standard.

### Limites connues (non résolues, hors scope actuel)

- **Pas d'authentification / multi-utilisateur** : tous les projets créés sont visibles par tous les visiteurs de l'instance déployée — acceptable pour une démo portfolio, à corriger avant tout usage réel (isolation par utilisateur, ou a minima par session anonyme).
- **Pas de vérification de compilation automatique** du code backend généré.
- **Templates Angular inline** : les composants générés utilisent `template: \`...\`` plutôt que des fichiers `.html` séparés.

---

## 🗺 Roadmap / améliorations possibles

- [ ] Authentification utilisateur (isolation des projets par compte)
- [ ] Vérification automatique de compilation du code généré (`dotnet build` en sandbox)
- [ ] Tests unitaires sur le parsing des sorties IA et la logique d'orchestration
- [ ] Génération de composants Angular avec fichiers séparés (`.ts` / `.html` / `.scss`)
- [ ] Diagramme ERD généré visuellement à partir de la sortie de l'Architecte
- [ ] Nom de domaine personnalisé + certificat SSL

---

## 📄 Licence

Projet personnel à but pédagogique et de portfolio.

---

*Construit avec .NET, Angular, Semantic Kernel et beaucoup de débogage Azure.*
