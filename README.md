# FleetRent Desktop

Système professionnel de gestion de location de véhicules — application desktop Windows.

## Fonctionnalités

- Gestion des agences, véhicules, catégories de véhicules
- Gestion des clients et permis de conduire
- Réservations avec détection des chevauchements
- Contrats de location (départ, retour, clôture)
- Paiements, cautions, remboursements
- Dommages et entretiens
- Tableau de bord et rapports (revenus, utilisation, rentabilité)
- Authentification locale et autorisation par rôles
- Journalisation et audit des opérations

## Stack technique

C# · .NET · WPF · XAML · MVVM · Clean Architecture · Entity Framework Core · SQL · xUnit

## Architecture

Le projet suit les principes de la Clean Architecture, avec une règle de dépendance stricte : le domaine ne dépend de rien d'autre.

```
FleetRent.Wpf            → Views, ViewModels, MVVM
        ↓
FleetRent.Application     → cas d'utilisation, DTO, interfaces
        ↓
FleetRent.Domain          → entités, règles métier, objets-valeurs
        ↑
FleetRent.Infrastructure  → EF Core, SQL, repositories
```

## Structure du projet

```
src/
├── FleetRent.Domain/
├── FleetRent.Application/
├── FleetRent.Infrastructure/
└── FleetRent.Wpf/
tests/
├── FleetRent.Domain.Tests/
├── FleetRent.Application.Tests/
├── FleetRent.Infrastructure.Tests/
└── FleetRent.Wpf.Tests/
```

## Prérequis

- .NET SDK 10
- Windows (WPF)

## Installation

```bash
git clone https://github.com/<ton-user>/FleetRent.git
cd FleetRent
dotnet restore
```

## Lancer l'application

```bash
dotnet run --project src/FleetRent.Wpf
```

## Lancer les tests

```bash
dotnet test
```

## Roadmap

- [x] Phase 1 — Cadrage
- [x] Phase 2 — Architecture (solution, projets, dépendances)
- [ ] Phase 3 — Domaine
- [ ] Phase 4 — Application
- [ ] Phase 5 — Infrastructure
- [ ] Phase 6 — Fondations WPF
- [ ] Phase 7 — Modules principaux
- [ ] Phase 8 — Administration
- [ ] Phase 9 — Rapports
- [ ] Phase 10 — Qualité (tests, sécurité, installateur)

## Licence

Ce projet est sous licence MIT — voir le fichier [LICENSE](LICENSE).
