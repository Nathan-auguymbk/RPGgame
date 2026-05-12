using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.Services;

/// <summary>
/// SRP : uniquement la prise de décision ennemie.
/// DIP : dépend de IExecuteurCompetence, pas de la classe concrète.
/// </summary>
public sealed class IAMonstre
{
    private static readonly Random _rng = Random.Shared;
    private readonly IExecuteurCompetence _executeur;

    private static readonly Competence _attaque =
        new("Attaque", "⚔", TypeCompetence.Physique, TypeCible.UnEnnemi, 16);

    private static readonly Competence _defense =
        new("Défense", "🛡", TypeCompetence.Defendre, TypeCible.Soi, 0);

    // DIP : reçoit l'interface, pas l'implémentation
    public IAMonstre(IExecuteurCompetence executeur) => _executeur = executeur;

    public void Agir(Monstre monstre, List<Heros> heroes,
        Action<string, string> journal)
    {
        var vivants = heroes.Where(h => h.EstEnVie).ToList();
        if (vivants.Count == 0) return;

        monstre.EnDefense = false;

        // Défendre si HP critique (< 30%)
        if ((float)monstre.HP / monstre.MaxHP < 0.3f && _rng.NextDouble() < 0.2f)
        {
            _executeur.Executer(monstre, new List<Personnage> { monstre }, _defense, journal);
            return;
        }

        var cible = vivants[_rng.Next(vivants.Count)];
        _executeur.Executer(monstre, new List<Personnage> { cible }, _attaque, journal);
    }
}
