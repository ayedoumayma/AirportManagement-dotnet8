# ✈️ Airport Management System — .NET 8

> **Projet académique** réalisé dans le cadre du cours **.NET / C#** (Semestre 7).  
> Application complète de gestion des activités d'un aéroport, couvrant la modélisation objet, LINQ, Entity Framework Core et une interface console interactive.

---

## � Aperçu

Ce projet simule la gestion d'un aéroport : avions, vols, passagers (voyageurs et personnel), billets et réservations. Il a été développé progressivement en plusieurs parties, chacune introduisant un nouveau concept du développement .NET :

| # | Concept couvert |
|---|---|
| 1 | Modélisation objet & diagramme de classes |
| 2 | Instanciation, constructeurs, initialiseurs |
| 3 | Polymorphisme de surcharge |
| 4 | Polymorphisme d'héritage |
| 5 | Interfaces, services, données de test |
| 6 | LINQ (query syntax & lambda expressions) |
| 7 | Méthodes d'extension |
| 8–9 | Entity Framework Core — DbContext & migrations |
| 10 | Data Annotations |
| 11 | Fluent API & configuration avancée |
| 12 | Table porteuse de données & CRUD complet |

---

## 🏗️ Architecture

La solution suit une architecture **3-couches** propre :

```
AirportManagement.sln
├── AM.ApplicationCore      → Domaine métier : entités, interfaces, services
├── AM.Infrastructure       → Couche données : EF Core DbContext, configurations, migrations
└── AM.UI.Console           → Interface utilisateur console interactive
```

```
AM.UI.Console
      │
      ▼
AM.Infrastructure
      │
      ▼
AM.ApplicationCore
```

---

## 🗂️ Modèle de données

```
PlaneType (enum)
  Boing | Airbus

┌─────────────┐   1      *   ┌──────────────────┐
│    Plane    │──────────────│      Flight       │
│─────────────│              │──────────────────│
│ PlaneId     │              │ FlightId          │
│ PlaneType   │              │ Airline           │
│ Capacity    │              │ Departure         │
│ Manufacture │              │ Destination       │
│   Date      │              │ FlightDate        │
└─────────────┘              │ EstimatedDuration │
                             │ EffectiveArrival  │
                             └────────┬──────────┘
                                      │ * (many-to-many)
                                      │  via table "Reservation"
                                      │ *
                             ┌────────┴──────────┐
                             │     Passenger      │
                             │───────────────────│
                             │ Id                │
                             │ PassportNumber    │
                             │ FullName          │
                             │ BirthDate         │
                             │ EmailAddress      │
                             │ TelNumber         │
                             └────────┬──────────┘
                                      │ (TPH inheritance)
                          ┌───────────┴───────────┐
                   ┌──────┴──────┐         ┌──────┴──────┐
                   │    Staff    │         │  Traveller  │
                   │─────────────│         │─────────────│
                   │ Function    │         │HealthInfo   │
                   │EmployDate   │         │ Nationality │
                   │ Salary      │         └─────────────┘
                   └─────────────┘

Ticket (table porteuse de données)
  PK composée : (PassengerFK, FlightFK, NumTicket)
  Prix | Siege | VIP
```

### Relations EF Core

| Relation | Type | Détail |
|---|---|---|
| `Plane` → `Flight` | One-to-Many | FK `planeId`, `ClientSetNull` on delete |
| `Flight` ↔ `Passenger` | Many-to-Many | Table de jointure `Reservation` |
| `Passenger` → `Ticket` | One-to-Many | FK `PassengerFK` |
| `Flight` → `Ticket` | One-to-Many | FK `FlightFK` |
| `Passenger` / `Staff` / `Traveller` | Héritage TPH | Discriminateur `IsTraveler` : `0` / `1` / `2` |

---

## 🔍 Fonctionnalités LINQ (ServiceFlight)

| Méthode | Description |
|---|---|
| `GetFlightDates(destination)` | Retourne les dates de vols vers une destination — implémenté en `for`, `foreach` puis LINQ |
| `GetFlights(filterType, filterValue)` | Filtrage dynamique des vols par attribut |
| `ShowFlightDetails(plane)` | Affiche les dates et destinations des vols d'un avion |
| `ProgrammedFlightNumber(startDate)` | Nombre de vols programmés sur 7 jours |
| `DurationAverage(destination)` | Moyenne des durées estimées par destination |
| `OrderedDurationFlights()` | Vols triés par durée décroissante |
| `SeniorTravellers(flight)` | Les 3 voyageurs les plus âgés d'un vol |
| `DestinationGroupedFlights()` | Vols groupés et affichés par destination |

> Chaque méthode est disponible en **query syntax** et **lambda expression**.  
> Des délégués `Action<Plane>` et `Func<string, double>` avec méthodes anonymes sont également utilisés.

---

## ⚙️ Configuration EF Core (Fluent API)

| Classe de configuration | Ce qu'elle configure |
|---|---|
| `PlaneConfiguration` | Table `MyPlanes`, colonne `Capacity` → `PlaneCapacity` |
| `FlightConfiguration` | Relation many-to-many → table `Reservation`, FK `planeId` avec `ClientSetNull` |
| `PassengerConfiguration` | Héritage TPH, `FullName` en owned entity (`PassFirstName` / `PassLastName`) |
| `TicketConfiguration` | Clé primaire composée `(PassengerFK, FlightFK, NumTicket)` |

Convention globale : toutes les propriétés `DateTime` → type SQL `datetime2`.

---

## 🖥️ Application Console

L'application propose un menu interactif à 8 options :

```
=== Airport Management (Console) ===
1. Ajouter un Avion
2. Ajouter un Vol
3. Ajouter un Voyageur
4. Affecter des Vols à un Avion
5. Affecter des Voyageurs à des Vols
6. Afficher le nombre de vols par avion
7. Afficher le nombre de passagers par vol
8. Quitter
```

Un mode **démo automatique** est disponible via `--demo` pour tester les insertions sans interaction.

---

## 🚀 Lancement

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server LocalDB (inclus avec Visual Studio 2022)

### Étapes

```bash
# 1. Cloner le dépôt
git clone https://github.com/ayedoumayma/AirportManagement-dotnet8.git
cd AirportManagement-dotnet8

# 2. Restaurer les packages NuGet
dotnet restore

# 3. Créer la base de données (migrations EF Core)
dotnet ef database update --project AM.Infrastructure --startup-project AM.UI.Console

# 4. Lancer l'application interactive
dotnet run --project AM.UI.Console

# 5. Ou lancer le mode démo (insertions automatiques)
dotnet run --project AM.UI.Console -- --demo
```

---

## � Stack technique

| Technologie | Version |
|---|---|
| .NET / C# | 8.0 |
| Entity Framework Core | 8.0.13 |
| EF Core SqlServer | 8.0.13 |
| EF Core Proxies (Lazy Loading) | 8.0.13 |
| SQL Server LocalDB | — |

---

## 📁 Structure du projet

```
AirportManagement/
│
├── AM.ApplicationCore/
│   ├── Domain/
│   │   ├── Plane.cs
│   │   ├── Flight.cs
│   │   ├── Passenger.cs
│   │   ├── Staff.cs
│   │   ├── Traveller.cs
│   │   ├── FullName.cs
│   │   ├── Ticket.cs
│   │   └── TestData.cs
│   ├── Interfaces/
│   │   └── IServiceFlight.cs
│   └── Services/
│       ├── ServiceFlight.cs
│       └── PassengerExtension.cs
│
├── AM.Infrastructure/
│   ├── AMContext.cs
│   ├── Configurations/
│   │   ├── PlaneConfiguration.cs
│   │   ├── FlightConfiguration.cs
│   │   ├── PassangerConfiguration.cs
│   │   └── TicketConfiguration.cs
│   └── Migrations/
│
└── AM.UI.Console/
    ├── Program.cs
    ├── MenuActions.cs
    ├── ConsoleHelpers.cs
    └── Demo.cs
```

---

## 🎓 Compétences couvertes

Ce projet couvre de manière progressive et appliquée les compétences suivantes :

### 🔷 POO & C#
| Compétence | Détail |
|---|---|
| Héritage | `Staff` et `Traveller` héritent de `Passenger` |
| Polymorphisme de surcharge | `CheckProfile(string, string)` et `CheckProfile(string, string, string)` |
| Polymorphisme d'héritage | `PassengerType()` virtuelle, surchargée dans chaque sous-classe |
| Interfaces | `IServiceFlight` définit le contrat, `ServiceFlight` l'implémente |
| Méthodes d'extension | `PassengerExtension.UpperFullName(this Passenger p)` |
| Délégués & méthodes anonymes | `Action<Plane>` et `Func<string, double>` initialisés avec des lambdas |

### 🔷 LINQ
| Compétence | Détail |
|---|---|
| Query syntax | `from f in Flights where ... select ...` |
| Lambda expressions | `.Where().Select().OrderBy()...` |
| Opérateurs utilisés | `Where`, `Select`, `OrderBy`, `OrderByDescending`, `GroupBy`, `Average`, `Count`, `Take`, `OfType` |
| Projections | Types anonymes `new { f.FlightDate, f.Destination }` |
| Évolution | Chaque méthode implémentée d'abord en `for`, puis `foreach`, puis LINQ |

### 🔷 Entity Framework Core 8
| Compétence | Détail |
|---|---|
| Code First + Migrations | Modèle C# → base de données via `Add-Migration` / `Update-Database` |
| `DbContext` & `DbSet` | `AMContext` avec 6 DbSets |
| Lazy Loading Proxies | `UseLazyLoadingProxies()` + propriétés `virtual` |
| Relation One-to-Many | `Plane` → `Flight` avec FK `planeId` et `ClientSetNull` |
| Relation Many-to-Many | `Flight` ↔ `Passenger` via table `Reservation` |
| Héritage TPH | Table unique `Passengers` avec discriminateur `IsTraveler` (0/1/2) |
| Data Annotations | `[StringLength]`, `[Range]`, `[DataType]`, `[Display]`, `[ForeignKey]` |
| Fluent API | `IEntityTypeConfiguration<T>` pour `Plane`, `Flight`, `Passenger`, `Ticket` |
| Owned Entity | `FullName` (FirstName + LastName) mappé en colonnes `PassFirstName` / `PassLastName` |
| Clé composite | `Ticket` avec PK `(PassengerFK, FlightFK, NumTicket)` |
| Pré-conventions | `ConfigureConventions()` → toutes les colonnes `DateTime` en `datetime2` |

### 🔷 Architecture
| Compétence | Détail |
|---|---|
| Architecture 3-couches | `ApplicationCore` (domaine) / `Infrastructure` (données) / `UI.Console` (présentation) |
| Séparation des responsabilités | Chaque couche a un rôle unique et des dépendances unidirectionnelles |
| Pattern Repository-like | `ServiceFlight` encapsule toute la logique métier sur les vols |

---

## 👩‍💻 Contexte académique

Projet réalisé dans le cadre d'un cours de développement **.NET / C#** (Semestre 7).  
Chaque partie du cours (théorie) a été appliquée directement dans ce projet, servant de support pratique consolidé pour l'ensemble de la promotion.
