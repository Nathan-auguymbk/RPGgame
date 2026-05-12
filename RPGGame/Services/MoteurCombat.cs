using RPGGame.Interfaces;
using RPGGame.Models;
using RPGGame.UI;

namespace RPGGame.Services;

/// <summary>
/// SRP strict : orchestre UNIQUEMENT le flux du combat.
/// Ne lit pas l'input (→ ResolveurAction).
/// N'affiche pas (→ Affichage).
/// DIP : dépend de IExecuteurCompetence, IAMonstre, ResolveurAction.
/// </summary>
public sealed class MoteurCombat : IMoteurCombat
{
    private static readonly Random _rng = Random.Shared;

    private readonly IExecuteurCompetence _executeur;
    private readonly IAMonstre            _ia;
    private readonly ResolveurAction      _resolver;

    // DIP : toutes les dépendances injectées, aucun "new" interne
    public MoteurCombat(IExecuteurCompetence executeur,
                        IAMonstre ia,
                        ResolveurAction resolver)
    {
        _executeur = executeur;
        _ia        = ia;
        _resolver  = resolver;
    }

    public ResultatCombat LancerCombat(List<Heros> heroes,
        List<Monstre> monstres, List<Objet> inventaire)
    {
        var ordre  = BuildInitiative(heroes, monstres);
        var logs   = new List<(string msg, string tag)>();
        int tour   = 0;
        int round  = 1;

        void Journal(string msg, string tag) => logs.Add((msg, tag));

        Journal($"\n  === COMBAT COMMENCE ===", "system");
        Journal($"  Initiative : {string.Join(" → ", ordre.Select(p => p.Nom))}", "info");

        while (true)
        {
            if (!heroes.Any(h => h.EstEnVie))  return ResultatCombat.Defaite;
            if (!monstres.Any(m => m.EstEnVie)) return ResultatCombat.Victoire;

            // Nouveau round
            if (tour > 0 && tour % ordre.Count == 0)
            {
                round++;
                Journal($"\n  ─── Tour {round} ───", "system");
                AppliquerPoison(heroes, monstres, Journal);
                if (!heroes.Any(h => h.EstEnVie))  { ViderLogs(logs); return ResultatCombat.Defaite;  }
                if (!monstres.Any(m => m.EstEnVie)) { ViderLogs(logs); return ResultatCombat.Victoire; }
            }

            var acteur = ordre[tour % ordre.Count];
            tour++;

            if (!acteur.EstEnVie) continue;

            if (acteur is Heros heros)
            {
                Console.Clear();
                Affichage.BanniereCompacte();
                Affichage.EtatCombat(heroes, monstres, round, heros);
                ViderLogs(logs);

                bool ok = false;
                while (!ok)
                    ok = _resolver.Resoudre(heros, heroes, monstres, inventaire, Journal);
            }
            else if (acteur is Monstre monstre)
            {
                Journal($"\n  [ Tour de {monstre.Icone} {monstre.Nom} ]", "system");
                _ia.Agir(monstre, heroes, Journal);
            }

            // Log des morts (une seule fois)
            LoguerMorts(heroes, monstres, logs, Journal);
            ViderLogs(logs);

            if (!heroes.Any(h => h.EstEnVie))  return ResultatCombat.Defaite;
            if (!monstres.Any(m => m.EstEnVie)) return ResultatCombat.Victoire;
        }
    }

    // ── Helpers privés ────────────────────────────────────────────────────────

    private static void AppliquerPoison(List<Heros> heroes, List<Monstre> monstres,
        Action<string, string> journal)
    {
        var tous = heroes.Cast<Personnage>().Concat(monstres);
        foreach (var p in tous.Where(p => p.EstEnVie && p.AStatut(EffetStatut.Poison)))
        {
            int dmg = _rng.Next(6, 15);
            p.SubirDegats(dmg);
            journal($"  ☠  {p.Nom} subit {dmg} dégâts de poison !", "statut");
        }
    }

    private static void LoguerMorts(List<Heros> heroes, List<Monstre> monstres,
        List<(string, string)> logs, Action<string, string> journal)
    {
        foreach (var h in heroes.Where(h => !h.EstEnVie))
            if (!logs.Any(l => l.Item1.Contains(h.Nom) && l.Item1.Contains("vaincu")))
                journal($"  💔 {h.Nom} est vaincu !", "mort");

        foreach (var m in monstres.Where(m => !m.EstEnVie))
            if (!logs.Any(l => l.Item1.Contains(m.Nom) && l.Item1.Contains("vaincu")))
                journal($"  💀 {m.Nom} est vaincu !", "mort");
    }

    private static void ViderLogs(List<(string msg, string tag)> logs)
    {
        foreach (var (msg, tag) in logs)
            Affichage.MessageJournal(msg, tag);
        logs.Clear();
    }

    private static List<Personnage> BuildInitiative(List<Heros> heroes, List<Monstre> monstres)
        => heroes.Cast<Personnage>()
                 .Concat(monstres)
                 .OrderByDescending(p => p.Agilite)
                 .ThenBy(_ => _rng.Next())
                 .ToList();
}
