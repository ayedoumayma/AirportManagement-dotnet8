using AM.ApplicationCore.Domain;
using AM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AM.UI.Console;

public static class MenuActions
{
    public static void AjouterAvion(AMContext ctx)
    {
        System.Console.WriteLine("=== Ajouter un Avion ===");

        System.Console.WriteLine("Type d'avion:");
        System.Console.WriteLine("1. Boing");
        System.Console.WriteLine("2. Airbus");
        var t = ConsoleHelpers.ReadInt("Choix", min: 1, max: 2);

        var capacity = ConsoleHelpers.ReadInt("Capacité", min: 1);
        var manufactureDate = ConsoleHelpers.ReadDateTime("Date de fabrication");

        var plane = new Plane
        {
            PlaneType = t == 1 ? PlaneType.Boing : PlaneType.Airbus,
            Capacity = capacity,
            ManufactureDate = manufactureDate
        };

        ctx.Set<Plane>().Add(plane);
        ctx.SaveChanges();

        System.Console.WriteLine($"OK. Avion inséré. PlaneId = {plane.PlaneId}");
        PrintDbCounts(ctx);
    }

    public static void AjouterVol(AMContext ctx)
    {
        System.Console.WriteLine("=== Ajouter un Vol ===");

        var planes = ListPlanes(ctx);
        if (planes.Count == 0)
        {
            System.Console.WriteLine("Aucun avion en base. Ajoutez d'abord un avion (option 1).");
            return;
        }

        var planeId = ConsoleHelpers.ReadInt("PlaneId (avion)", min: 1);
        if (!planes.Any(p => p.PlaneId == planeId))
        {
            System.Console.WriteLine("PlaneId introuvable.");
            return;
        }

        var airline = ConsoleHelpers.ReadString("Airline", required: false);
        var departure = ConsoleHelpers.ReadString("Departure", required: true);
        var destination = ConsoleHelpers.ReadString("Destination", required: true);
        var flightDate = ConsoleHelpers.ReadDateTime("Date du vol");
        var estimatedDuration = ConsoleHelpers.ReadInt("Durée estimée (minutes)", min: 0);
        var effectiveArrival = ConsoleHelpers.ReadDateTime("Arrivée effective");

        var flight = new Flight
        {
            Airline = airline,
            Departure = departure,
            Destination = destination,
            FlightDate = flightDate,
            EstimatedDuration = estimatedDuration,
            EffectiveArrival = effectiveArrival,
            planeId = planeId
        };

        ctx.Set<Flight>().Add(flight);
        ctx.SaveChanges();

        System.Console.WriteLine($"OK. Vol inséré. FlightId = {flight.FlightId}");
        PrintDbCounts(ctx);
    }

    public static void AjouterVoyageur(AMContext ctx)
    {
        System.Console.WriteLine("=== Ajouter un Voyageur ===");

        var passport = ConsoleHelpers.ReadString("Numéro de passeport", required: true, maxLength: 7);
        var firstName = ConsoleHelpers.ReadString("Prénom", required: true, maxLength: 30);
        var lastName = ConsoleHelpers.ReadString("Nom", required: true);
        var birthDate = ConsoleHelpers.ReadDateTime("Date de naissance");
        var tel = ConsoleHelpers.ReadString("Téléphone", required: false);
        var email = ConsoleHelpers.ReadString("Email", required: false);
        var health = ConsoleHelpers.ReadString("HealthInformation", required: false);
        var nationality = ConsoleHelpers.ReadString("Nationality", required: false);

        var traveller = new Traveller
        {
            PassportNumber = passport,
            FullName = new FullName { FirstName = firstName, LastName = lastName },
            BirthDate = birthDate,
            TelNumber = tel,
            EmailAddress = email,
            HealthInformation = health,
            Nationality = nationality
        };

        ctx.Set<Traveller>().Add(traveller);
        ctx.SaveChanges();

        System.Console.WriteLine($"OK. Voyageur inséré. Id = {traveller.Id}");
        PrintDbCounts(ctx);
    }

    public static void AffecterVolsAvion(AMContext ctx)
    {
        System.Console.WriteLine("=== Affecter des Vols à un Avion ===");

        var planes = ListPlanes(ctx);
        if (planes.Count == 0)
        {
            System.Console.WriteLine("Aucun avion en base.");
            return;
        }

        var flights = ListFlights(ctx);
        if (flights.Count == 0)
        {
            System.Console.WriteLine("Aucun vol en base.");
            return;
        }

        var planeId = ConsoleHelpers.ReadInt("PlaneId (avion)", min: 1);
        if (!planes.Any(p => p.PlaneId == planeId))
        {
            System.Console.WriteLine("PlaneId introuvable.");
            return;
        }

        System.Console.WriteLine("Entrez les FlightId à affecter (séparés par des virgules), ex: 1,2,3");
        var idsRaw = ConsoleHelpers.ReadString("FlightIds", required: true);
        var ids = ParseIds(idsRaw);
        if (ids.Count == 0)
        {
            System.Console.WriteLine("Aucun id valide.");
            return;
        }

        var flightsToUpdate = ctx.Set<Flight>().Where(f => ids.Contains(f.FlightId)).ToList();
        foreach (var f in flightsToUpdate)
            f.planeId = planeId;

        ctx.SaveChanges();

        System.Console.WriteLine($"OK. {flightsToUpdate.Count} vol(s) affecté(s) à l'avion {planeId}.");
        System.Console.WriteLine($"Vérif: Nb vols pour cet avion = {ctx.Set<Flight>().Count(f => f.planeId == planeId)}");
    }

    public static void AffecterVoyageursVol(AMContext ctx)
    {
        System.Console.WriteLine("=== Affecter des Voyageurs à un Vol ===");

        var flights = ListFlights(ctx);
        if (flights.Count == 0)
        {
            System.Console.WriteLine("Aucun vol en base.");
            return;
        }

        var travellers = ListTravellers(ctx);
        if (travellers.Count == 0)
        {
            System.Console.WriteLine("Aucun voyageur en base.");
            return;
        }

        var flightId = ConsoleHelpers.ReadInt("FlightId", min: 1);

        var flight = ctx.Set<Flight>()
            .Include(f => f.Passengers)
            .FirstOrDefault(f => f.FlightId == flightId);

        if (flight is null)
        {
            System.Console.WriteLine("FlightId introuvable.");
            return;
        }

        System.Console.WriteLine("Entrez les Id voyageurs à affecter (séparés par des virgules), ex: 10,12");
        var idsRaw = ConsoleHelpers.ReadString("TravellerIds", required: true);
        var ids = ParseIds(idsRaw);
        if (ids.Count == 0)
        {
            System.Console.WriteLine("Aucun id valide.");
            return;
        }

        var passengersToAdd = ctx.Set<Traveller>().Where(t => ids.Contains(t.Id)).ToList();
        var added = 0;
        foreach (var p in passengersToAdd)
        {
            var already = flight.Passengers.Any(x => x.Id == p.Id);
            if (already) continue;
            flight.Passengers.Add(p);
            added++;
        }

        ctx.SaveChanges();

        System.Console.WriteLine($"OK. {added} voyageur(s) ajouté(s) au vol {flightId}.");
        System.Console.WriteLine($"Vérif: Nb passagers du vol = {ctx.Set<Flight>().Where(f => f.FlightId == flightId).Select(f => f.Passengers.Count).First()}");
    }

    public static void NbVolsParAvion(AMContext ctx)
    {
        System.Console.WriteLine("=== Nombre de vols par Avion ===");
        ListPlanes(ctx);

        var planeId = ConsoleHelpers.ReadInt("PlaneId (avion)", min: 1);
        var nb = ctx.Set<Flight>().Count(f => f.planeId == planeId);
        System.Console.WriteLine($"Avion {planeId} -> {nb} vol(s).");
    }

    public static void NbPassagersParVol(AMContext ctx)
    {
        System.Console.WriteLine("=== Nombre de passagers pour chaque vol ===");

        var rows = ctx.Set<Flight>()
            .AsNoTracking()
            .Select(f => new
            {
                f.FlightId,
                f.Departure,
                f.Destination,
                f.FlightDate,
                PassengersCount = f.Passengers.Count
            })
            .OrderBy(r => r.FlightId)
            .ToList();

        if (rows.Count == 0)
        {
            System.Console.WriteLine("Aucun vol en base.");
            return;
        }

        foreach (var r in rows)
        {
            System.Console.WriteLine($"FlightId={r.FlightId} | {r.Departure} -> {r.Destination} | {r.FlightDate:yyyy-MM-dd HH:mm} | Passagers={r.PassengersCount}");
        }
    }

    private static void PrintDbCounts(AMContext ctx)
    {
        var planes = ctx.Set<Plane>().Count();
        var flights = ctx.Set<Flight>().Count();
        var passengers = ctx.Set<Passenger>().Count();
        var travellers = ctx.Set<Traveller>().Count();

        System.Console.WriteLine($"Vérification DB (counts): Planes={planes} | Flights={flights} | Passengers(total)={passengers} | Travellers={travellers}");
    }

    private static List<Plane> ListPlanes(AMContext ctx)
    {
        var planes = ctx.Set<Plane>()
            .AsNoTracking()
            .OrderBy(p => p.PlaneId)
            .ToList();

        System.Console.WriteLine("--- Avions ---");
        if (planes.Count == 0)
        {
            System.Console.WriteLine("(vide)");
            return planes;
        }

        foreach (var p in planes)
        {
            System.Console.WriteLine($"PlaneId={p.PlaneId} | Type={p.PlaneType} | Capacité={p.Capacity} | Fab={p.ManufactureDate:yyyy-MM-dd}");
        }

        return planes;
    }

    private static List<Flight> ListFlights(AMContext ctx)
    {
        var flights = ctx.Set<Flight>()
            .AsNoTracking()
            .OrderBy(f => f.FlightId)
            .ToList();

        System.Console.WriteLine("--- Vols ---");
        if (flights.Count == 0)
        {
            System.Console.WriteLine("(vide)");
            return flights;
        }

        foreach (var f in flights)
        {
            System.Console.WriteLine($"FlightId={f.FlightId} | {f.Departure} -> {f.Destination} | {f.FlightDate:yyyy-MM-dd HH:mm} | PlaneId={f.planeId}");
        }

        return flights;
    }

    private static List<Traveller> ListTravellers(AMContext ctx)
    {
        var travellers = ctx.Set<Traveller>()
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToList();

        System.Console.WriteLine("--- Voyageurs ---");
        if (travellers.Count == 0)
        {
            System.Console.WriteLine("(vide)");
            return travellers;
        }

        foreach (var t in travellers)
        {
            var fn = t.FullName?.FirstName ?? "";
            var ln = t.FullName?.LastName ?? "";
            System.Console.WriteLine($"Id={t.Id} | {fn} {ln} | Passport={t.PassportNumber} | Nat={t.Nationality}");
        }

        return travellers;
    }

    private static HashSet<int> ParseIds(string raw)
    {
        var set = new HashSet<int>();
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(part, out var id) && id > 0)
                set.Add(id);
        }

        return set;
    }
}

