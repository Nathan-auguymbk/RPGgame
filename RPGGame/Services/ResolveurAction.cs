using RPGGame.Interfaces;
using RPGGame.Models;
using RPGGame.UI;

namespace RPGGame.Services;

/// <summary>
/// SRP : traduit les choix du joueur en actions de combat.
/// Extrait de MoteurCombat pour respecter Single Responsibility.
/// DIP : dépend de IExecuteurCompetence et IInputHandler.
/// </summary>
public sealed class ResolveurAction
{
    private readonly IExecuteurCompetence _executeur;
    private readonly IInputHandler        _input;

    public ResolveurAction(IExecuteurCompetence executeur, IInputHandler input)
    {
        _executeur = executeur;
        _input     = input;
    }

    /// <summary>
    /// Demande une action au joueur et l'exécute.
    /// Retourne false si le joueur annule (retour au menu).
    /// </summary>
    public bool Resoudre(Heros heros, List<Heros> heroes,
        List<Monstre> monstres, List<Objet> inventaire,
        Action<string, string> journal)
    {
        heros.EnDefense = false;

        Affichage.MenuAction(heros.Nom);
        int choix = _input.LireEntier(1, 4);

        return choix switch
        {
            1 => ExecuterAttaque(heros, monstres, journal),
            2 => ExecuterCompetence(heros, heroes, monstres, journal),
            3 => ExecuterObjet(heros, heroes, inventaire, journal),
            4 => ExecuterDefense(heros, journal),
            _ => false
        };
    }

    // ── Actions ──────────────────────────────────────────────────────────────

    private bool ExecuterAttaque(Heros heros, List<Monstre> monstres,
        Action<string, string> journal)
    {
        var comp   = heros.Competences.First(c => c.Type == TypeCompetence.Physique);
        var cible  = ChoisirEnnemi(monstres);
        if (cible == null) return false;
        _executeur.Executer(heros, new List<Personnage> { cible }, comp, journal);
        return true;
    }

    private bool ExecuterCompetence(Heros heros, List<Heros> heroes,
        List<Monstre> monstres, Action<string, string> journal)
    {
        Affichage.MenuCompetences(heros);
        int choix = _input.LireEntier(0, heros.Competences.Count);
        if (choix == 0) return false;

        var comp = heros.Competences[choix - 1];
        if (!heros.PeutUtiliser(comp)) { Affichage.ErreurMana(); return false; }

        var cibles = ResoudreCibles(comp, heros, heroes, monstres);
        if (cibles == null) return false;

        _executeur.Executer(heros, cibles, comp, journal);
        return true;
    }

    private bool ExecuterObjet(Heros heros, List<Heros> heroes,
        List<Objet> inventaire, Action<string, string> journal)
    {
        var dispos = inventaire.Where(o => o.Disponible).ToList();
        if (dispos.Count == 0) { Affichage.ErreurInventaireVide(); return false; }

        Affichage.MenuObjets(dispos);
        int co = _input.LireEntier(0, dispos.Count);
        if (co == 0) return false;

        var objet   = dispos[co - 1];
        var vivants = heroes.Where(h => h.EstEnVie).ToList();
        Affichage.MenuCibles(vivants.Cast<Personnage>().ToList(), "allié");
        int ct = _input.LireEntier(0, vivants.Count);
        if (ct == 0) return false;

        _executeur.UtiliserObjet(heros, vivants[ct - 1], objet, journal);
        return true;
    }

    private bool ExecuterDefense(Heros heros, Action<string, string> journal)
    {
        var comp = new Competence("Défense", "🛡", TypeCompetence.Defendre, TypeCible.Soi, 0);
        _executeur.Executer(heros, new List<Personnage> { heros }, comp, journal);
        return true;
    }

    // ── Résolution des cibles ─────────────────────────────────────────────────

    private List<Personnage>? ResoudreCibles(Competence comp, Heros acteur,
        List<Heros> heroes, List<Monstre> monstres)
    {
        return comp.Cible switch
        {
            TypeCible.UnEnnemi    => ChoisirEnnemi(monstres) is { } e
                                        ? new List<Personnage> { e } : null,
            TypeCible.TousEnnemis => monstres.Where(m => m.EstEnVie).Cast<Personnage>().ToList(),
            TypeCible.UnAllie     => ChoisirAllie(heroes) is { } a
                                        ? new List<Personnage> { a } : null,
            TypeCible.TousAllies  => heroes.Where(h => h.EstEnVie).Cast<Personnage>().ToList(),
            TypeCible.Soi         => new List<Personnage> { acteur },
            _                     => null
        };
    }

    private Personnage? ChoisirEnnemi(List<Monstre> monstres)
    {
        var vivants = monstres.Where(m => m.EstEnVie).ToList();
        if (vivants.Count == 0) return null;
        if (vivants.Count == 1) return vivants[0];
        Affichage.MenuCibles(vivants.Cast<Personnage>().ToList(), "ennemi");
        int c = _input.LireEntier(0, vivants.Count);
        return c == 0 ? null : vivants[c - 1];
    }

    private Personnage? ChoisirAllie(List<Heros> heroes)
    {
        var vivants = heroes.Where(h => h.EstEnVie).ToList();
        if (vivants.Count == 0) return null;
        if (vivants.Count == 1) return vivants[0];
        Affichage.MenuCibles(vivants.Cast<Personnage>().ToList(), "allié");
        int c = _input.LireEntier(0, vivants.Count);
        return c == 0 ? null : vivants[c - 1];
    }
}
