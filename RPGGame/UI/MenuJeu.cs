using RPGGame.Data;
using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.UI;

/// <summary>
/// SRP : navigation entre les menus uniquement.
/// DIP : dépend de IMoteurCombat et IInputHandler, jamais des concrets.
/// </summary>
public sealed class MenuJeu
{
    private readonly IMoteurCombat  _moteur;
    private readonly RegistreClasses _registre;
    private readonly DonneesMonstres _monstres;
    private readonly IInputHandler  _input;
    private readonly List<Heros>    _groupe = new();

    // DIP : tout est injecté
    public MenuJeu(IMoteurCombat moteur, RegistreClasses registre,
                   DonneesMonstres monstres, IInputHandler input)
    {
        _moteur   = moteur;
        _registre = registre;
        _monstres = monstres;
        _input    = input;
    }

    public void DemarrerJeu()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        while (true)
        {
            Affichage.MenuPrincipal();
            int c = _input.LireEntier(1, 2);
            if (c == 2) { Console.WriteLine("\n  À bientôt !\n"); return; }
            BoucleJeu();
        }
    }

    private void BoucleJeu()
    {
        _groupe.Clear();
        CreerEquipe();
        if (_groupe.Count == 0) return;

        while (true)
        {
            var monstres  = _monstres.GenererRencontre(_groupe.Count);
            var inventaire = _monstres.InventaireDeBase();

            Console.Clear();
            Affichage.BannierePrincipale();
            Console.WriteLine($"  Rencontre : {string.Join(", ", monstres.Select(m => m.Nom))}");
            _input.Attendre("Appuyez sur ENTRÉE pour lancer le combat...");

            var resultat = _moteur.LancerCombat(_groupe, monstres, inventaire);

            if (resultat == ResultatCombat.Victoire)
            {
                int xp = monstres.Sum(m => m.Recompense);
                Affichage.EcranVictoire(_groupe, xp);
                Console.WriteLine($"  {"\x1b[93m"}[1] Nouveau combat   [2] Menu principal{"\x1b[0m"}");
                if (_input.LireEntier(1, 2) == 2) return;
                foreach (var h in _groupe.Where(h => h.EstEnVie))
                {
                    h.RecevoirSoins(h.MaxHP / 4);
                    h.RestaurerMP(h.MaxMP / 4);
                }
                _input.Attendre();
            }
            else
            {
                Affichage.EcranDefaite();
                _input.Attendre();
                return;
            }
        }
    }

    private void CreerEquipe()
    {
        while (true)
        {
            Affichage.TitreCreation();
            Affichage.GroupeActuel(_groupe);

            Console.WriteLine($"  {"\x1b[93m"}[1]{"\x1b[0m"} Ajouter un héros");
            if (_groupe.Count > 0)
            {
                Console.WriteLine($"  {"\x1b[93m"}[2]{"\x1b[0m"} Retirer un héros");
                Console.WriteLine($"  {"\x1b[93m"}[3]{"\x1b[0m"} Lancer l'aventure");
            }
            Console.WriteLine($"  {"\x1b[90m"}[0] Menu principal{"\x1b[0m"}\n");

            int max   = _groupe.Count > 0 ? 3 : 1;
            int choix = _input.LireEntier(0, max);

            switch (choix)
            {
                case 0: return;
                case 1: AjouterHeros(); break;
                case 2: RetirerHeros(); break;
                case 3: if (_groupe.Count > 0) return; break;
            }
        }
    }

    private void AjouterHeros()
    {
        if (_groupe.Count >= 4)
        {
            Console.WriteLine($"\n  {"\x1b[91m"}Équipe complète (4/4).{"\x1b[0m"}");
            _input.Attendre(); return;
        }
        Affichage.TitreCreation();
        var defs = _registre.ToutesLesClasses().ToList();
        Affichage.MenuClasses(defs);

        int c      = _input.LireEntier(1, defs.Count);
        var def    = defs[c - 1];
        string nom = _input.LireNom($"Nom du {def.Classe}");
        var heros  = _registre.Creer(nom, def.Classe);
        _groupe.Add(heros);

        Console.WriteLine($"\n  {"\x1b[92m"}✓ {heros.Icone} {heros.Nom} ({heros.Classe}) rejoint l'équipe !{"\x1b[0m"}");
        _input.Attendre();
    }

    private void RetirerHeros()
    {
        Affichage.GroupeActuel(_groupe);
        Console.WriteLine($"  {"\x1b[91m"}Retirer ? [0=Annuler]{"\x1b[0m"}");
        for (int i = 0; i < _groupe.Count; i++)
            Console.WriteLine($"  [{i + 1}] {_groupe[i].Nom}");
        Console.WriteLine();

        int c = _input.LireEntier(0, _groupe.Count);
        if (c == 0) return;
        string nom = _groupe[c - 1].Nom;
        _groupe.RemoveAt(c - 1);
        Console.WriteLine($"  {"\x1b[91m"}{nom} a quitté l'équipe.{"\x1b[0m"}");
        _input.Attendre();
    }
}
