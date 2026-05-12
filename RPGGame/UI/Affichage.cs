using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.UI;

/// <summary>
/// SRP : affichage console uniquement. Zéro logique métier, zéro input.
/// Reste statique car c'est de la présentation pure sans état.
/// </summary>
public static class Affichage
{
    private const string RST   = "\x1b[0m";
    private const string GRAS  = "\x1b[1m";
    private const string OR    = "\x1b[93m";
    private const string ORSOM = "\x1b[33m";
    private const string ROUGE = "\x1b[91m";
    private const string ROUGES= "\x1b[31m";
    private const string CYAN  = "\x1b[96m";
    private const string VERT  = "\x1b[92m";
    private const string VIO   = "\x1b[95m";
    private const string BLEUP = "\x1b[94m";
    private const string GRIS  = "\x1b[90m";
    private const string BLANC = "\x1b[97m";

    public static void BannierePrincipale()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine($"  {OR}╔══════════════════════════════════════════╗{RST}");
        Console.WriteLine($"  {OR}║  {GRAS}  C H R O N I C L E S   O F   F A T E{RST}{OR}  ║{RST}");
        Console.WriteLine($"  {OR}║         {GRIS}Système de Combat RPG{RST}{OR}             ║{RST}");
        Console.WriteLine($"  {OR}╚══════════════════════════════════════════╝{RST}");
        Console.WriteLine();
    }

    public static void BanniereCompacte()
        => Console.WriteLine($"  {OR}── Chronicles of Fate ─────────────────────────{RST}\n");

    public static void MenuPrincipal()
    {
        BannierePrincipale();
        Console.WriteLine($"  {OR}[1]{RST} ⚔  Nouvelle aventure");
        Console.WriteLine($"  {GRIS}[2]    Quitter{RST}");
        Console.WriteLine();
    }

    public static void TitreCreation()
    {
        BannierePrincipale();
        Console.WriteLine($"  {OR}╔══ CRÉER VOTRE ÉQUIPE ══╗{RST}\n");
    }

    public static void MenuClasses(IEnumerable<IDefinitionClasse> definitions)
    {
        Console.WriteLine($"  {OR}Choisir une classe :{RST}");
        int i = 1;
        foreach (var d in definitions)
        {
            string col = CouleurClasse(d.Classe);
            Console.WriteLine($"  {col}[{i}] {d.Icone}  {d.Classe,-10}{RST} {GRIS}{d.Description}{RST}");
            i++;
        }
        Console.WriteLine();
    }

    public static void GroupeActuel(List<Heros> heroes)
    {
        Console.WriteLine($"  {OR}── Équipe ({heroes.Count}/4) ─────────────────────{RST}");
        if (heroes.Count == 0)
            Console.WriteLine($"  {GRIS}  (aucun héros){RST}");
        else
            foreach (var h in heroes)
            {
                string col = CouleurClasse(h.Classe);
                Console.WriteLine($"  {col}{h.Icone} {h.Nom,-12}{RST}" +
                    $" {GRIS}{h.Classe,-9}{RST}" +
                    $"  PV:{VERT}{h.MaxHP,3}{RST}" +
                    $"  PM:{BLEUP}{h.MaxMP,3}{RST}" +
                    $"  FRC:{ROUGE}{h.Force,2}{RST}" +
                    $"  AGI:{OR}{h.Agilite,2}{RST}");
            }
        Console.WriteLine();
    }

    public static void EtatCombat(List<Heros> heroes, List<Monstre> monstres,
        int round, Heros actif)
    {
        Console.WriteLine($"  {OR}══ TOUR {round} ══════════════════════════════════════{RST}\n");

        Console.WriteLine($"  {ROUGE}── Ennemis ───────────────────────────────────{RST}");
        for (int i = 0; i < monstres.Count; i++)
        {
            var m = monstres[i];
            Console.Write($"  [{i + 1}] {ROUGE}{m.Icone} {m.Nom,-14}{RST}");
            BarreVie(m.HP, m.MaxHP);
            Console.Write($"  {GRIS}FRC:{m.Force} DEF:{m.Defense} AGI:{m.Agilite}{RST}");
            Statuts(m);
            if (!m.EstEnVie) Console.Write($" {GRIS}[✗]{RST}");
            Console.WriteLine();
        }

        Console.WriteLine($"\n  {CYAN}── Équipe ────────────────────────────────────{RST}");
        foreach (var h in heroes)
        {
            bool actifFlag = h == actif;
            string col     = actifFlag ? OR : CouleurClasse(h.Classe);
            Console.Write($"  {col}{h.Icone} {h.Nom,-12}{RST}");
            BarreVie(h.HP, h.MaxHP);
            BarreMP(h.MP, h.MaxMP);
            Statuts(h);
            if (actifFlag) Console.Write($" {OR}◄{RST}");
            if (!h.EstEnVie) Console.Write($" {GRIS}[✗]{RST}");
            Console.WriteLine();
        }
        Console.WriteLine($"\n  {OR}──────────────────────────────────────────────{RST}");
    }

    public static void MenuAction(string nom)
    {
        Console.WriteLine($"\n  {OR}Tour de {GRAS}{nom}{RST}{OR} :{RST}");
        Console.WriteLine($"  {ROUGE}[1]{RST} ⚔  Attaquer");
        Console.WriteLine($"  {VIO}[2]{RST} ✦  Compétences");
        Console.WriteLine($"  {VERT}[3]{RST} P  Objets");
        Console.WriteLine($"  {BLEUP}[4]{RST} 🛡  Défendre\n");
    }

    public static void MenuCompetences(Heros h)
    {
        Console.WriteLine($"\n  {OR}── Compétences ────────────────────────────────{RST}");
        for (int i = 0; i < h.Competences.Count; i++)
        {
            var c   = h.Competences[i];
            bool ok = h.PeutUtiliser(c);
            string type = c.Type switch
            {
                TypeCompetence.Magique  => $"{VIO}[MAG]{RST}",
                TypeCompetence.Soin     => $"{VERT}[SOIN]{RST}",
                TypeCompetence.Buff     => $"{OR}[BUFF]{RST}",
                TypeCompetence.Defendre => $"{BLEUP}[DEF]{RST}",
                _                       => $"{ROUGE}[PHY]{RST}",
            };
            string cibles = c.Cible switch
            {
                TypeCible.TousEnnemis => " (Tous ennemis)",
                TypeCible.TousAllies  => " (Tous alliés)",
                TypeCible.UnAllie     => " (Allié)",
                TypeCible.Soi         => " (Soi)",
                _                     => ""
            };
            string col = ok ? BLANC : GRIS;
            Console.WriteLine($"  [{i + 1}] {type} {col}{c.Icone} {c.Nom,-20}{RST}" +
                              $" PM:{BLEUP}{c.CoutMP,2}{RST}" +
                              $" P:{ROUGE}{c.Puissance,2}{RST}" +
                              $"{GRIS}{cibles}{RST}");
        }
        Console.WriteLine($"  {GRIS}[0] Retour{RST}\n");
    }

    public static void MenuObjets(List<Objet> objets)
    {
        Console.WriteLine($"\n  {OR}── Objets ─────────────────────────────────────{RST}");
        for (int i = 0; i < objets.Count; i++)
        {
            var o = objets[i];
            Console.WriteLine($"  [{i + 1}] {VERT}{o.Icone} {o.Nom,-22}{RST}" +
                              $" x{OR}{o.Quantite}{RST}" +
                              (o.Puissance > 0 ? $" [{VERT}+{o.Puissance}{RST}]" : ""));
        }
        Console.WriteLine($"  {GRIS}[0] Retour{RST}\n");
    }

    public static void MenuCibles(List<Personnage> cibles, string type)
    {
        string col = type == "ennemi" ? ROUGE : CYAN;
        Console.WriteLine($"\n  {col}── Choisir un {type} ──────────────────────────{RST}");
        for (int i = 0; i < cibles.Count; i++)
        {
            var c = cibles[i];
            Console.Write($"  [{i + 1}] {col}{c.Icone} {c.Nom,-14}{RST}");
            BarreVie(c.HP, c.MaxHP);
            Console.WriteLine();
        }
        Console.WriteLine($"  {GRIS}[0] Retour{RST}\n");
    }

    public static void EcranVictoire(List<Heros> heroes, int xp)
    {
        Console.Clear();
        Console.WriteLine($"\n  {OR}╔══════════════════════════════════════════╗");
        Console.WriteLine($"  ║        {GRAS}{VERT}  ★  VICTOIRE !  ★{RST}{OR}              ║");
        Console.WriteLine($"  ╚══════════════════════════════════════════╝{RST}\n");
        Console.WriteLine($"  {OR}── Bilan ──────────────────────────────────────{RST}");
        int xpH = heroes.Count > 0 ? xp / heroes.Count : 0;
        foreach (var h in heroes)
        {
            string s = h.EstEnVie ? $"{VERT}Vivant{RST}" : $"{ROUGE}Vaincu{RST}";
            Console.WriteLine($"  {CouleurClasse(h.Classe)}{h.Icone} {h.Nom,-12}{RST}" +
                $"  {s}  PV:{VERT}{h.HP}/{h.MaxHP}{RST}" +
                $"  Dégâts:{ROUGE}{h.TotalDegats,5}{RST}" +
                $"  Soins:{VERT}{h.TotalSoins,4}{RST}" +
                $"  {OR}+{xpH}xp{RST}");
        }
        Console.WriteLine();
    }

    public static void EcranDefaite()
    {
        Console.Clear();
        Console.WriteLine($"\n  {ROUGE}╔══════════════════════════════════════════╗");
        Console.WriteLine($"  ║          {GRAS}  ✗  DÉFAITE  ✗{RST}{ROUGE}               ║");
        Console.WriteLine($"  ║       Tous vos héros sont tombés...      ║");
        Console.WriteLine($"  ╚══════════════════════════════════════════╝{RST}\n");
    }

    public static void MessageJournal(string msg, string tag)
    {
        string col = tag switch
        {
            "heros"    => CYAN,
            "monstre"  => ROUGE,
            "soin"     => VERT,
            "statut"   => VIO,
            "critique" => $"{GRAS}{OR}",
            "mort"     => ROUGES,
            "info"     => OR,
            "system"   => ORSOM,
            "erreur"   => ROUGE,
            _          => GRIS,
        };
        Console.WriteLine($"{col}{msg}{RST}");
    }

    public static void ErreurMana()
        => Console.WriteLine($"\n  {ROUGE}  ✗ Mana insuffisant !{RST}\n");

    public static void ErreurInventaireVide()
        => Console.WriteLine($"\n  {ROUGE}  ✗ Inventaire vide !{RST}\n");

    // ── Barres ────────────────────────────────────────────────────────────────

    private static void BarreVie(int val, int max)
    {
        float p = max > 0 ? (float)val / max : 0;
        string c = p > 0.5f ? VERT : p > 0.25f ? OR : ROUGE;
        int r = (int)(p * 12);
        Console.Write($" {c}[{new string('|', r)}{new string(' ', 12 - r)}]{RST}" +
                      $" {BLANC}{val,3}/{max,-3}{RST}");
    }

    private static void BarreMP(int val, int max)
    {
        if (max <= 0) return;
        int r = (int)((float)val / max * 8);
        Console.Write($" {BLEUP}[{new string(':', r)}{new string(' ', 8 - r)}]{RST}" +
                      $" {GRIS}PM{val}/{max}{RST}");
    }

    private static void Statuts(Personnage p)
    {
        if (p.AStatut(EffetStatut.Poison))  Console.Write($" {VIO}[☠]{RST}");
        if (p.AStatut(EffetStatut.Ralenti)) Console.Write($" {CYAN}[❄]{RST}");
        if (p.EnDefense)                    Console.Write($" {BLEUP}[🛡]{RST}");
    }

    // ── Couleurs par classe ────────────────────────────────────────────────────

    private static string CouleurClasse(TypeClasse c) => c switch
    {
        TypeClasse.Guerrier => ROUGE,
        TypeClasse.Mage     => VIO,
        TypeClasse.Voleur   => VERT,
        TypeClasse.Clerc    => OR,
        _                   => BLANC
    };
}
