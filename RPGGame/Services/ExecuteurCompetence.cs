using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.Services;

/// <summary>
/// Résout la stratégie correcte et l'exécute.
/// DIP : reçoit le dictionnaire de stratégies par injection.
/// ISP : implémente IExecuteurCompetence, interface minimale.
/// </summary>
public sealed class ExecuteurCompetence : IExecuteurCompetence
{
    private readonly IReadOnlyDictionary<TypeCompetence, ICompetenceStrategy> _strategies;

    // DIP : les stratégies arrivent de Program.cs, pas instanciées ici
    public ExecuteurCompetence(IReadOnlyDictionary<TypeCompetence, ICompetenceStrategy> strategies)
    {
        _strategies = strategies;
    }

    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence competence, Action<string, string> journal)
    {
        if (!acteur.ConsommerMP(competence.CoutMP))
        {
            journal($"  ✗ {acteur.Nom} n'a pas assez de mana !", "erreur");
            return;
        }

        if (!_strategies.TryGetValue(competence.Type, out var strategie))
        {
            journal($"  ? Stratégie inconnue pour : {competence.Type}", "erreur");
            return;
        }

        strategie.Executer(acteur, cibles, competence, journal);
    }

    public void UtiliserObjet(Personnage acteur, Personnage cible,
        Objet objet, Action<string, string> journal)
    {
        if (!objet.Disponible) { journal("  ✗ Objet épuisé !", "erreur"); return; }
        objet.Quantite--;

        switch (objet.Type)
        {
            case TypeObjet.Soin:
                cible.RecevoirSoins(objet.Puissance);
                acteur.TotalSoins += objet.Puissance;
                journal($"  {objet.Icone} {acteur.Nom} utilise {objet.Nom} sur {cible.Nom}  +{objet.Puissance} PV", "soin");
                break;

            case TypeObjet.Mana:
                int restaure = cible.RestaurerMP(objet.Puissance);
                journal($"  {objet.Icone} {acteur.Nom} utilise {objet.Nom}  +{restaure} PM", "info");
                break;

            case TypeObjet.Antidote:
                cible.RetirerStatut(EffetStatut.Poison);
                journal($"  {objet.Icone} Poison de {cible.Nom} guéri !", "soin");
                break;
        }
    }
}
