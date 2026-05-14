using AM.ApplicationCore.Domain;
using AM.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AM.UI.Console;

public static class Demo
{
    public static void Run(AMContext ctx)
    {
        System.Console.WriteLine("=== DEMO (non-interactif) : insertions + vérification DB ===");

        // 1) Ajouter un avion
        var plane = new Plane
        {
            PlaneType = PlaneType.Airbus,
            Capacity = 180,
            ManufactureDate = DateTime.Today.AddYears(-3)
        };
        ctx.Set<Plane>().Add(plane);
        ctx.SaveChanges();

        // 2) Ajouter un vol rattaché à cet avion
        var flight = new Flight
        {
            Airline = "DemoAir",
            Departure = "TUN",
            Destination = "PAR",
            FlightDate = DateTime.Now.AddDays(7),
            EstimatedDuration = 150,
            EffectiveArrival = DateTime.Now.AddDays(7).AddMinutes(150),
            planeId = plane.PlaneId
        };
        ctx.Set<Flight>().Add(flight);
        ctx.SaveChanges();

        // 3) Ajouter un voyageur
        var passport = (DateTime.UtcNow.Ticks % 10_000_000).ToString("D7");
        var traveller = new Traveller
        {
            PassportNumber = passport,
            FullName = new FullName { FirstName = "Demo", LastName = "Traveller" },
            BirthDate = new DateTime(1995, 1, 1),
            TelNumber = "00000000",
            EmailAddress = "demo.traveller@demo.local",
            HealthInformation = "RAS",
            Nationality = "TN"
        };
        ctx.Set<Traveller>().Add(traveller);
        ctx.SaveChanges();

        // 5) Affecter voyageur -> vol (table Reservation)
        var loadedFlight = ctx.Set<Flight>()
            .Include(f => f.Passengers)
            .First(f => f.FlightId == flight.FlightId);

        loadedFlight.Passengers.Add(traveller);
        ctx.SaveChanges();

        // 6) Nb vols par avion
        var nbVols = ctx.Set<Flight>().Count(f => f.planeId == plane.PlaneId);

        // 7) Nb passagers par vol (pour le vol demo)
        var nbPassagersVol = ctx.Set<Flight>()
            .Where(f => f.FlightId == flight.FlightId)
            .Select(f => f.Passengers.Count)
            .First();

        System.Console.WriteLine($"OK. PlaneId={plane.PlaneId}, FlightId={flight.FlightId}, TravellerId={traveller.Id}, Passport={passport}");
        System.Console.WriteLine($"Vérif 6: Avion {plane.PlaneId} -> {nbVols} vol(s).");
        System.Console.WriteLine($"Vérif 7: Vol {flight.FlightId} -> {nbPassagersVol} passager(s).");

        // Counts globaux
        System.Console.WriteLine($"Counts DB: Planes={ctx.Set<Plane>().Count()} | Flights={ctx.Set<Flight>().Count()} | Travellers={ctx.Set<Traveller>().Count()} | Passengers(total)={ctx.Set<Passenger>().Count()}");
        System.Console.WriteLine("=== FIN DEMO ===");
    }
}

