using RPGGame.Interfaces;

namespace RPGGame.UI;

/// <summary>
/// Implémentation concrète de IInputHandler pour la console.
/// DIP : c'est l'unique endroit où Console.ReadLine est appelé pour l'input.
/// En test, on injecte un FakeInputHandler à la place → zéro modification.
/// </summary>
public sealed class ConsoleInputHandler : IInputHandler
{
    private const string RST   = "\x1b[0m";
    private const string BLANC = "\x1b[97m";
    private const string ROUGE = "\x1b[91m";
    private const string GRIS  = "\x1b[90m";

    public int LireEntier(int min, int max)
    {
        while (true)
        {
            Console.Write($"  {BLANC}>{RST} ");
            string? saisie = Console.ReadLine()?.Trim();
            if (int.TryParse(saisie, out int val) && val >= min && val <= max)
                return val;
            Console.WriteLine($"  {ROUGE}Entrez un nombre entre {min} et {max}.{RST}");
        }
    }

    public string LireNom(string invite)
    {
        while (true)
        {
            Console.Write($"  {invite}{BLANC}: {RST}");
            string? nom = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(nom) && nom.Length <= 14) return nom;
            Console.WriteLine($"  {ROUGE}Nom invalide (1-14 caractères).{RST}");
        }
    }

    public void Attendre(string message = "Appuyez sur ENTRÉE pour continuer...")
    {
        Console.WriteLine($"\n  {GRIS}{message}{RST}");
        Console.ReadLine();
    }
}
