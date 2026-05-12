using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.Strategies;

// Toutes les formules de combat sont ici.
// Les strategies NE connaissent PAS MoteurCombat, MenuJeu, Affichage.
// Elles reçoivent ce dont elles ont besoin → pas de dépendance cachée.

public sealed class StrategiePhysique : ICompetenceStrategy
{
    private static readonly Random _rng = Random.Shared;

    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence comp, Action<string, string> journal)
    {
        foreach (var cible in cibles)
        {
            if (!cible.EstEnVie) continue;
            for (int i = 0; i < comp.NombreCoups; i++)
            {
                int reduction = cible.EnDefense ? cible.Defense * 2 : cible.Defense;
                int dmg       = Math.Max(1, acteur.Force + comp.Puissance + _rng.Next(-4, 7) - reduction);

                bool critique = comp.ChanceCrit > 0f && _rng.NextDouble() < comp.ChanceCrit;
                if (critique) dmg = (int)(dmg * 1.9f);

                cible.SubirDegats(dmg);
                acteur.TotalDegats += dmg;

                journal(
                    critique
                        ? $"  CRITIQUE ! {acteur.Icone} {acteur.Nom} → {cible.Nom}  -{dmg} PV"
                        : $"  {comp.Icone} {acteur.Nom} utilise {comp.Nom} sur {cible.Nom}  -{dmg} PV",
                    critique ? "critique" : acteur.EstHeros ? "heros" : "monstre");
            }

            if (comp.Effet == EffetStatut.Poison && cible.EstEnVie)
            {
                cible.AjouterStatut(EffetStatut.Poison);
                journal($"  ☠  {cible.Nom} est empoisonné !", "statut");
            }
        }
    }
}

public sealed class StrategieMagique : ICompetenceStrategy
{
    private static readonly Random _rng = Random.Shared;

    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence comp, Action<string, string> journal)
    {
        foreach (var cible in cibles)
        {
            if (!cible.EstEnVie) continue;

            int res = cible.EnDefense ? (int)(cible.ResistanceMagique * 1.6f) : cible.ResistanceMagique;
            int dmg = Math.Max(1, acteur.Intelligence + comp.Puissance + _rng.Next(-3, 6) - res);

            cible.SubirDegats(dmg);
            acteur.TotalDegats += dmg;
            journal($"  {comp.Icone} {acteur.Nom} lance {comp.Nom} sur {cible.Nom}  -{dmg} PV",
                acteur.EstHeros ? "heros" : "monstre");

            if (comp.Effet == EffetStatut.Aucun && comp.Nom.Contains("Drain"))
            {
                int absorbe = dmg / 2;
                acteur.RecevoirSoins(absorbe);
                journal($"     ↳ {acteur.Nom} absorbe {absorbe} PV", "soin");
            }

            if (comp.Effet == EffetStatut.Ralenti && cible.EstEnVie)
            {
                cible.AjouterStatut(EffetStatut.Ralenti);
                journal($"  ❄  {cible.Nom} est ralenti !", "statut");
            }
        }
    }
}

public sealed class StrategieSoin : ICompetenceStrategy
{
    private static readonly Random _rng = Random.Shared;

    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence comp, Action<string, string> journal)
    {
        foreach (var cible in cibles)
        {
            if (!cible.EstEnVie) continue;
            int montant = comp.Puissance + acteur.Intelligence / 2 + _rng.Next(0, 11);
            cible.RecevoirSoins(montant);
            acteur.TotalSoins += montant;
            journal($"  {comp.Icone} {acteur.Nom} soigne {cible.Nom}  +{montant} PV", "soin");
        }
    }
}

public sealed class StrategieBuff : ICompetenceStrategy
{
    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence comp, Action<string, string> journal)
    {
        acteur.AugmenterForce(4);
        journal($"  {comp.Icone} {acteur.Nom} utilise {comp.Nom} ! Force +4.", "info");
    }
}

public sealed class StrategieDefendre : ICompetenceStrategy
{
    public void Executer(Personnage acteur, List<Personnage> cibles,
        Competence comp, Action<string, string> journal)
    {
        acteur.EnDefense = true;
        journal($"  🛡  {acteur.Nom} se met en position défensive.", "info");
    }
}
