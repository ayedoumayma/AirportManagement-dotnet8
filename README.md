# ✈️ Airport Management — .NET 8 Academic Project

> Projet académique réalisé dans le cadre du cours **.NET / C#** à **ESPRIT School of Engineering**.

---

## 📋 Description

Application de gestion des activités d'un aéroport développée en **C# / .NET 8**, couvrant l'ensemble du cycle de développement : modélisation objet, LINQ, Entity Framework Core, migrations, Data Annotations et Fluent API.

---

## 🏗️ Architecture de la solution

La solution suit une architecture **3-couches** :

```
AirportManagement.sln
├── AM.ApplicationCore      → Domaine métier (entités, interfaces, services)
├── AM.Infrastructure       → Couche données (EF Core DbContext, configurations)
└── AM.UI.Console           → Interface utilisateur console
```

**Dépendances entre projets :**
```
AM.UI.Console → AM.Infrastructure → AM.ApplicationCore
```

---

## 📦 Technologies & Packages

| Package | Version | Projet(s) |
|---|---|---|
| Microsoft.EntityFrameworkCore | 8.0.13 | Tous |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.13 | Infrastructure, UI |
| Microsoft.EntityFrameworkCore.Tools | 8.0.13 | Infrastructure, UI |
| Microsoft.EntityFrameworkCore.Design | 8.0.13 | Infrastructure, UI |
| Microsoft.EntityFrameworkCore.Proxies | 8.0.13 | ApplicationCore, Infrastructure |

**Base de données :** SQL Server LocalDB (`AirportMgmtDB`)

---

## 🗂️ Modèle de données

### Diagramme de classes

```
PlaneType (enum)
  └── Boing | Airbus

Plane ──────────────────────── 1 ──── * ──── Flight
  - PlaneId (PK)                              - FlightId (PK)
  - PlaneType                                 - Airline
  - Capacity                                  - Departure / Destination
  - ManufactureDate                           - FlightDate / EffectiveArrival
                                              - EstimatedDuration
                                              - planeId (FK)
                                                    │
                                              * ────┤ (many-to-many via Reservation)
                                                    │
Passenger ◄──────────────────────────────────────── *
  - Id (PK)
  - PassportNumber
  - FullName (owned: FirstName, LastName)
  - BirthDate / TelNumber / EmailAddress
       ▲
       ├── Staff
       │     - Function / EmployementDate / Salary
       └── Traveller
             - HealthInformation / Nationality

Ticket (table porteuse de données)
  - PK composée : (PassengerFK, FlightFK, NumTicket)
  - Prix / Siege / VIP
```

### Relations
| Relation | Type | Table/Clé |
|---|---|---|
| Plane → Flight | One-to-Many | FK `planeId` (ClientSetNull) |
| Flight ↔ Passenger | Many-to-Many | Table `Reservation` |
| Passenger → Ticket | One-to-Many | FK `PassengerFK` |
| Flight → Ticket | One-to-Many | FK `FlightFK` |
| Passenger (TPH) | Héritage | Discriminateur `IsTraveler` (0/1/2) |

---

## 🧩 Parties implémentées

### Partie 1 — Diagramme de classes
- Implémentation de toutes les entités du diagramme
- Héritage `Passenger` → `Staff` / `Traveller`
- Propriétés de navigation avec mot-clé `virtual`
- Override de `ToString()` sur toutes les classes

### Partie 2 — Instanciation des objets
- Constructeur non paramétré, constructeur paramétré, initialiseurs d'objets

### Partie 3 — Polymorphisme de surcharge
- Méthodes `CheckProfile(string, string)` et `CheckProfile(string, string, string)` dans `Passenger`

### Partie 4 — Polymorphisme d'héritage
- Méthode virtuelle `PassengerType()` surchargée dans `Staff` et `Traveller`

### Partie 5 — Interface & Service
- Interface `IServiceFlight` dans `AM.ApplicationCore/Interfaces`
- Classe `ServiceFlight` dans `AM.ApplicationCore/Services`
- Données de test statiques dans `TestData`

### Partie 5.1 & 5.2 — Itérations / Structures conditionnelles
- `GetFlightDates(string destination)` — version `for`, `foreach`, puis LINQ
- `GetFlights(string filterType, string filterValue)` — filtrage dynamique par attribut

### Partie 6 — LINQ
| Méthode | Description |
|---|---|
| `GetFlightDates(destination)` | Dates des vols vers une destination (LINQ query + lambda) |
| `ShowFlightDetails(Plane)` | Dates et destinations des vols d'un avion |
| `ProgrammedFlightNumber(DateTime)` | Nombre de vols sur 7 jours à partir d'une date |
| `DurationAverage(destination)` | Moyenne des durées estimées par destination |
| `OrderedDurationFlights()` | Vols triés par durée décroissante |
| `SeniorTravellers(Flight)` | 3 voyageurs les plus âgés d'un vol |
| `DestinationGroupedFlights()` | Vols groupés par destination |

Délégués `Action<Plane>` et `Func<string, double>` avec méthodes anonymes.

### Partie 7 — Méthodes d'extension
- `PassengerExtension.UpperFullName(this Passenger p)` — met en majuscule la première lettre du nom/prénom

### Partie 9 & 10 — Entity Framework Core
- Classe `AMContext` héritant de `DbContext`
- Lazy Loading Proxies activé (`UseLazyLoadingProxies()`)
- Convention globale : toutes les colonnes `DateTime` → type `datetime2`
- Migrations EF Core

### Partie 12 — Data Annotations
| Propriété | Annotation |
|---|---|
| `PassportNumber` | `[StringLength(7)]` |
| `BirthDate` | `[Display(Name="Date of birth")]`, `[DataType(DataType.Date)]` |
| `EmailAddress` | `[DataType(DataType.EmailAddress)]` |
| `Salary` | `[DataType(DataType.Currency)]` |
| `Capacity` | `[Range(0, int.MaxValue)]` |

### Partie 13 — Fluent API
- **PlaneConfiguration** : table `MyPlanes`, colonne `PlaneCapacity`
- **FlightConfiguration** : many-to-many → table `Reservation`, one-to-many avec `ClientSetNull`
- **PassengerConfiguration** : héritage TPH avec discriminateur `IsTraveler`, `FullName` en owned entity
- **TicketConfiguration** : clé primaire composée `(PassengerFK, FlightFK, NumTicket)`
- Pré-conventions : `ConfigureConventions()` pour forcer `datetime2`

### Partie 14 & 15 — Table porteuse de données & CRUD
- Entité `Ticket` avec clé composite
- Application console interactive (8 options) :
  1. Ajouter un Avion
  2. Ajouter un Vol
  3. Ajouter un Voyageur
  4. Affecter des Vols à un Avion
  5. Affecter des Voyageurs à des Vols
  6. Afficher le nombre de vols par avion
  7. Afficher le nombre de passagers par vol
  8. Quitter
- Mode `--demo` pour exécution automatisée sans interaction

---

## 🚀 Lancement

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server LocalDB (inclus avec Visual Studio)

### Démarrage

```bash
# Cloner le dépôt
git clone <url-du-repo>
cd AirportManagement_correctionS7/AirportManagement_correctionS7

# Restaurer les packages
dotnet restore

# Appliquer les migrations (crée la base AirportMgmtDB)
dotnet ef database update --project AM.Infrastructure --startup-project AM.UI.Console

# Lancer l'application interactive
dotnet run --project AM.UI.Console

# Ou lancer le mode démo automatique
dotnet run --project AM.UI.Console -- --demo
```

---

## 📁 Structure des fichiers

```
AM.ApplicationCore/
├── Domain/
│   ├── Plane.cs
│   ├── Flight.cs
│   ├── Passenger.cs
│   ├── Staff.cs
│   ├── Traveller.cs
│   ├── FullName.cs
│   ├── Ticket.cs
│   └── TestData.cs
├── Interfaces/
│   └── IServiceFlight.cs
└── Services/
    ├── ServiceFlight.cs
    └── PassengerExtension.cs

AM.Infrastructure/
├── AMContext.cs
└── Configurations/
    ├── PlaneConfiguration.cs
    ├── FlightConfiguration.cs
    ├── PassangerConfiguration.cs
    └── TicketConfiguration.cs

AM.UI.Console/
├── Program.cs
├── MenuActions.cs
├── ConsoleHelpers.cs
└── Demo.cs
```

---

## 👨‍🎓 Contexte académique

| | |
|---|---|
| **École** | ESPRIT School of Engineering |
| **Cours** | Développement .NET / C# |
| **Niveau** | Semestre 7 |
| **Framework** | .NET 8 |
