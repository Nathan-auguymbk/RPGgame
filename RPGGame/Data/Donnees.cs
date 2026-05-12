using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.Data;

/// <summary>
/// Registre des classes disponibles.
/// OCP : reçoit les définitions par injection → zéro switch, zéro if.
/// DIP : dépend de IDefinitionClasse, jamais des concrets.
/// </summary>
public sealed class RegistreClasses
{
    private readonly Dictionary<TypeClasse, IDefinitionClasse> _definitions;

    // DIP : les définitions arrivent de l'extérieur (Program.cs)
    public RegistreClasses(IEnumerable<IDefinitionClasse> definitions)
    {
        _definitions = definitions.ToDictionary(d => d.Classe);
    }

    public Heros Creer(string nom, TypeClasse classe)
    {
        if (!_definitions.TryGetValue(classe, out var def))
            throw new InvalidOperationException($"Classe non enregistrée : {classe}");
        return def.Creer(nom);
    }

    public IReadOnlyCollection<IDefinitionClasse> ToutesLesClasses()
        => _definitions.Values;
}

// ════════════════════════════════════════════════════════════════════════════

public sealed class DonneesMonstres
{
    private static readonly Random _rng = Random.Shared;

    // Les gabarits sont des données, pas de la logique → record immuable
    private record Gabarit(string Nom, string Icone,
        int HP, int Force, int Agi, int Def, int ResM, int XP);

    private readonly List<Gabarit> _gabarits =
    [
        new("Gobelin",    "G",  55,  10, 11,  4,  2, 10),
        new("Orc",        "O", 100,  16,  5, 10,  3, 22),
        new("Squelette",  "S",  68,  12,  9,  7,  9, 16),
        new("Vampire",    "V", 120,  18, 14,  9, 13, 40),
        new("Dragon",     "D", 240,  28,  8, 18, 22, 90),
        new("Loup Sombre","L",  78,  13, 18,  5,  3, 20),
    ];

    public Monstre CreerAleatoire()
    {
        var g = _gabarits[_rng.Next(_gabarits.Count)];
        return new Monstre(g.Nom, g.Icone, g.HP, g.Force, g.Agi, g.Def, g.ResM, g.XP);
    }

    public List<Monstre> GenererRencontre(int nbHeros)
    {
        int nb = _rng.Next(1, Math.Min(5, nbHeros + 2));
        return Enumerable.Range(0, nb).Select(_ => CreerAleatoire()).ToList();
    }

    public List<Objet> InventaireDeBase() =>
    [
        new("Potion de Soin", "P", TypeObjet.Soin,    65, 2),
        new("Potion de Mana", "M", TypeObjet.Mana,    45, 1),
        new("Antidote",       "A", TypeObjet.Antidote, 0, 1),
    ];
}
