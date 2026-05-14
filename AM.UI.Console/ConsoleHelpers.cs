using System.Globalization;

namespace AM.UI.Console;

public static class ConsoleHelpers
{
    public static int ReadInt(string label, int? min = null, int? max = null)
    {
        while (true)
        {
            System.Console.Write($"{label}: ");
            var input = System.Console.ReadLine();

            if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                System.Console.WriteLine("Valeur invalide (entier requis).");
                continue;
            }

            if (min.HasValue && value < min.Value)
            {
                System.Console.WriteLine($"Valeur invalide (min = {min}).");
                continue;
            }

            if (max.HasValue && value > max.Value)
            {
                System.Console.WriteLine($"Valeur invalide (max = {max}).");
                continue;
            }

            return value;
        }
    }

    public static double ReadDouble(string label, double? min = null, double? max = null)
    {
        while (true)
        {
            System.Console.Write($"{label}: ");
            var input = System.Console.ReadLine();

            if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                System.Console.WriteLine("Valeur invalide (nombre requis, ex: 120.5).");
                continue;
            }

            if (min.HasValue && value < min.Value)
            {
                System.Console.WriteLine($"Valeur invalide (min = {min}).");
                continue;
            }

            if (max.HasValue && value > max.Value)
            {
                System.Console.WriteLine($"Valeur invalide (max = {max}).");
                continue;
            }

            return value;
        }
    }

    public static bool ReadBool(string label)
    {
        while (true)
        {
            System.Console.Write($"{label} (o/n): ");
            var input = (System.Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (input is "o" or "oui" or "y" or "yes") return true;
            if (input is "n" or "non" or "no") return false;
            System.Console.WriteLine("Réponse invalide. Tapez o/oui ou n/non.");
        }
    }

    public static string ReadString(string label, bool required = true, int? maxLength = null)
    {
        while (true)
        {
            System.Console.Write($"{label}: ");
            var s = (System.Console.ReadLine() ?? "").Trim();

            if (required && string.IsNullOrWhiteSpace(s))
            {
                System.Console.WriteLine("Champ requis.");
                continue;
            }

            if (maxLength.HasValue && s.Length > maxLength.Value)
            {
                System.Console.WriteLine($"Trop long (max = {maxLength}).");
                continue;
            }

            return s;
        }
    }

    public static DateTime ReadDateTime(string label)
    {
        while (true)
        {
            System.Console.Write($"{label} (ex: 2026-03-05 14:30): ");
            var input = System.Console.ReadLine();

            if (DateTime.TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dt))
                return dt;

            if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt;

            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt;

            System.Console.WriteLine("Date invalide.");
        }
    }
}

