// ════════════════════════════════════════════════════════════════════════════
//
//   COMPOSITION ROOT — Le seul endroit du programme où "new" est autorisé.
//
//   C'est ici qu'on assemble les dépendances. Tout le reste du code
//   reçoit ses dépendances par constructeur → zéro couplage fort.
//
//   Pour tester MoteurCombat : injecter un FakeInputHandler.
//   Pour ajouter une classe  : ajouter ici, créer DefinitionXxx.cs.
//   Aucune autre ligne de code à toucher.
//
// ════════════════════════════════════════════════════════════════════════════

using RPGGame.Classes;
using RPGGame.Data;
using RPGGame.Interfaces;
using RPGGame.Models;
using RPGGame.Services;
using RPGGame.Strategies;
using RPGGame.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ── 1. Stratégies de compétences ─────────────────────────────────────────────
//    OCP : ajouter TypeCompetence.Invocation → ajouter une entrée ici + une classe
IReadOnlyDictionary<TypeCompetence, ICompetenceStrategy> strategies =
    new Dictionary<TypeCompetence, ICompetenceStrategy>
    {
        [TypeCompetence.Physique]  = new StrategiePhysique(),
        [TypeCompetence.Magique]   = new StrategieMagique(),
        [TypeCompetence.Soin]      = new StrategieSoin(),
        [TypeCompetence.Buff]      = new StrategieBuff(),
        [TypeCompetence.Defendre]  = new StrategieDefendre(),
    };

// ── 2. Services ───────────────────────────────────────────────────────────────
IExecuteurCompetence executeur = new ExecuteurCompetence(strategies);
IInputHandler        input     = new ConsoleInputHandler();
IAMonstre            ia        = new IAMonstre(executeur);
ResolveurAction      resolver  = new ResolveurAction(executeur, input);
IMoteurCombat        moteur    = new MoteurCombat(executeur, ia, resolver);

// ── 3. Données : registre des classes jouables ────────────────────────────────
//    OCP : pour ajouter Paladin → new DefinitionPaladin() ici, rien d'autre
RegistreClasses registre = new RegistreClasses(
[
    new DefinitionGuerrier(),
    new DefinitionMage(),
    new DefinitionVoleur(),
    new DefinitionClerc(),
]);

DonneesMonstres monstres = new DonneesMonstres();

// ── 4. Démarrage ──────────────────────────────────────────────────────────────
var jeu = new MenuJeu(moteur, registre, monstres, input);
jeu.DemarrerJeu();
