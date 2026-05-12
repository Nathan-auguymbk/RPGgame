using RPGGame.Models;

namespace RPGGame.Interfaces;

// ════════════════════════════════════════════════════════════════════════════
// OCP + DIP : stratégie d'exécution d'une compétence
// Nouvelle compétence = nouvelle implémentation. Aucun code existant modifié.
// ════════════════════════════════════════════════════════════════════════════
public interface ICompetenceStrategy
{
    void Executer(Personnage acteur, List<Personnage> cibles,
                  Competence competence, Action<string, string> journal);
}

// ════════════════════════════════════════════════════════════════════════════
// ISP : ce que MoteurCombat et IAMonstre ont besoin de savoir sur l'exécuteur
// Pas de détails d'implémentation exposés.
// ════════════════════════════════════════════════════════════════════════════
public interface IExecuteurCompetence
{
    void Executer(Personnage acteur, List<Personnage> cibles,
                  Competence competence, Action<string, string> journal);

    void UtiliserObjet(Personnage acteur, Personnage cible,
                       Objet objet, Action<string, string> journal);
}

// ════════════════════════════════════════════════════════════════════════════
// ISP + DIP : abstraction de la lecture d'entrée
// ConsoleInputHandler → en prod. FakeInputHandler → en test.
// ════════════════════════════════════════════════════════════════════════════
public interface IInputHandler
{
    int    LireEntier(int min, int max);
    string LireNom(string invite);
    void   Attendre(string message = "Appuyez sur ENTRÉE pour continuer...");
}

// ════════════════════════════════════════════════════════════════════════════
// DIP : MenuJeu dépend de cette abstraction, jamais de MoteurCombat
// ════════════════════════════════════════════════════════════════════════════
public interface IMoteurCombat
{
    ResultatCombat LancerCombat(List<Heros> heroes,
                                List<Monstre> monstres,
                                List<Objet> inventaire);
}

// ════════════════════════════════════════════════════════════════════════════
// OCP : ajouter une classe de personnage = implémenter cette interface.
// Aucun switch à modifier.
// ════════════════════════════════════════════════════════════════════════════
public interface IDefinitionClasse
{
    TypeClasse Classe { get; }
    string     Icone  { get; }
    string     Description { get; }
    Heros      Creer(string nom);
}
