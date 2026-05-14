using AM.ApplicationCore.Domain;
using AM.Infrastructure;
using AM.UI.Console;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;

using var ctx = new AMContext();

try
{
    ctx.Database.EnsureCreated();
}
catch (Exception ex)
{
    Console.WriteLine("Impossible de se connecter / créer la base de données.");
    Console.WriteLine(ex.Message);
    Console.WriteLine("Appuyez sur une touche pour quitter...");
    Console.ReadKey();
    return;
}

if (args.Any(a => a.Equals("--demo", StringComparison.OrdinalIgnoreCase)))
{
    Demo.Run(ctx);
    return;
}

while (true)
{
    Console.Clear();
    Console.WriteLine("=== Airport Management (Console) ===");
    Console.WriteLine("1. Ajouter un Avion");
    Console.WriteLine("2. Ajouter un Vol");
    Console.WriteLine("3. Ajouter un Voyageur");
    Console.WriteLine("4. Affecter des Vols à un Avion");
    Console.WriteLine("5. Affecter des Voyageurs à des Vols");
    Console.WriteLine("6. Afficher le nombre de vols assuré par un avion (id)");
    Console.WriteLine("7. Afficher le nombre de passagers pour chaque vol");
    Console.WriteLine("8. Quitter");
    Console.WriteLine();

    var choix = ConsoleHelpers.ReadInt("Votre choix", min: 1, max: 8);

    try
    {
        switch (choix)
        {
            case 1:
                MenuActions.AjouterAvion(ctx);
                break;
            case 2:
                MenuActions.AjouterVol(ctx);
                break;
            case 3:
                MenuActions.AjouterVoyageur(ctx);
                break;
            case 4:
                MenuActions.AffecterVolsAvion(ctx);
                break;
            case 5:
                MenuActions.AffecterVoyageursVol(ctx);
                break;
            case 6:
                MenuActions.NbVolsParAvion(ctx);
                break;
            case 7:
                MenuActions.NbPassagersParVol(ctx);
                break;
            case 8:
                return;
        }
    }
    catch (DbUpdateException dbEx)
    {
        Console.WriteLine("Erreur base de données (DbUpdateException).");
        Console.WriteLine(dbEx.InnerException?.Message ?? dbEx.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur: " + ex.Message);
    }

    Console.WriteLine();
    Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
    Console.ReadKey();
}
