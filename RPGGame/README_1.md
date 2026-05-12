# ⚔️ Chronicles of Fate — RPG Combat System

> Jeu de combat RPG au tour par tour en console, développé en **C# .NET 8**.  
> Architecture construite pour démontrer l'application réelle des **principes SOLID**.

---

## 🎮 Gameplay

- Créez une équipe de **1 à 4 héros** parmi 4 classes
- Affrontez des **monstres générés aléatoirement**
- Combat au tour par tour avec **ordre d'initiative** basé sur l'agilité
- Compétences, objets, effets de statut, IA ennemie

### Classes disponibles

| Classe | Icône | Rôle | Compétences |
|--------|-------|------|-------------|
| Guerrier | ⚔ | Tank physique | Frappe Lourde, Cri de Guerre, Défense |
| Mage | ✦ | DPS magique | Boule de Feu, Blizzard, Drain |
| Voleur | † | DPS agile | Coup Critique, Double Frappe, Lame Empoisonnée |
| Clerc | ✶ | Support/Heal | Soin, Soin de Groupe, Lumière Sacrée |

### Monstres

Gobelin · Orc · Squelette · Vampire · Dragon · Loup Sombre

### Effets de statut

- ☠ **Poison** — dégâts par tour
- ❄ **Ralenti** — appliqué par Blizzard
- 🛡 **Défense** — double la réduction de dégâts

---

## 🚀 Lancer le jeu

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Installation

```bash
git clone https://github.com/Nathan-auguymbk/RPGGame.git
cd RPGGame
dotnet run
```

---

## 🏗️ Architecture

```
RPGGame/
├── Program.cs                  ← Composition Root (seul endroit avec "new")
│
├── Models/                     ← État pur, zéro logique métier
│   ├── Personnage.cs           ← Classe abstraite de base
│   ├── Heros.cs
│   ├── Monstre.cs
│   ├── ValeurObjets.cs         ← Competence + Objet (immuables)
│   └── Enums.cs
│
├── Interfaces/                 ← Contrats : le cœur du découplage
│   └── Interfaces.cs           ← ICompetenceStrategy, IExecuteurCompetence,
│                                  IInputHandler, IMoteurCombat, IDefinitionClasse
│
├── Strategies/                 ← Formules de combat (Strategy pattern)
│   └── Strategies.cs           ← Physique, Magique, Soin, Buff, Défendre
│
├── Classes/                    ← Une définition par classe jouable
│   └── Definitions.cs          ← DefinitionGuerrier, DefinitionMage, ...
│
├── Data/                       ← Données statiques
│   └── Donnees.cs              ← RegistreClasses, DonneesMonstres
│
├── Services/                   ← Logique métier
│   ├── ExecuteurCompetence.cs  ← Résout et exécute les stratégies
│   ├── IAMonstre.cs            ← IA ennemie
│   ├── ResolveurAction.cs      ← Traduit l'input joueur en actions
│   └── MoteurCombat.cs         ← Orchestre le flux du combat
│
└── UI/                         ← Présentation uniquement
    ├── Affichage.cs            ← Rendu console avec couleurs ANSI
    ├── ConsoleInputHandler.cs  ← Lecture clavier (implémente IInputHandler)
    └── MenuJeu.cs              ← Navigation entre les menus
```

---

## 🔷 Principes SOLID appliqués

### S — Single Responsibility
Chaque classe a **une seule raison de changer** :

- `MoteurCombat` → orchestre le flux du combat uniquement
- `ResolveurAction` → lit les choix du joueur et les traduit en actions
- `Affichage` → rendu console, zéro logique métier
- `IAMonstre` → prise de décision ennemie uniquement

> La version précédente avait `MoteurCombat` qui gérait à la fois le combat ET l'input. Violation corrigée.

---

### O — Open/Closed
Le code est **ouvert à l'extension, fermé à la modification**.

**Ajouter une nouvelle compétence** → créer une classe `ICompetenceStrategy`, l'enregistrer dans `Program.cs`. Aucun fichier existant touché.

**Ajouter une nouvelle classe jouable** (ex: Paladin) :
1. Créer `Classes/DefinitionPaladin.cs`
2. Ajouter `new DefinitionPaladin()` dans `Program.cs`

C'est tout. Zéro `switch`, zéro `if` à modifier.

> La version précédente avait un `switch(classe)` dans `DonneesClasses.cs`. Violation corrigée.

---

### L — Liskov Substitution
`Heros` et `Monstre` héritent de `Personnage` et sont **interchangeables** partout où `Personnage` est attendu — sans changer le comportement attendu.

---

### I — Interface Segregation
**5 interfaces spécialisées**, chacune n'exposant que ce dont son consommateur a besoin :

| Interface | Consommateur | Ce qu'elle expose |
|-----------|-------------|-------------------|
| `ICompetenceStrategy` | `ExecuteurCompetence` | `Executer()` |
| `IExecuteurCompetence` | `MoteurCombat`, `IAMonstre` | `Executer()` + `UtiliserObjet()` |
| `IInputHandler` | `MenuJeu`, `ResolveurAction` | `LireEntier()`, `LireNom()`, `Attendre()` |
| `IMoteurCombat` | `MenuJeu` | `LancerCombat()` |
| `IDefinitionClasse` | `RegistreClasses` | `Creer()`, `Classe`, `Description` |

---

### D — Dependency Inversion
**`Program.cs` est la seule classe qui instancie des objets concrets.**

Tout le reste reçoit ses dépendances par constructeur :

```csharp
// Program.cs - Composition Root
IExecuteurCompetence executeur = new ExecuteurCompetence(strategies);
IInputHandler        input     = new ConsoleInputHandler();
IAMonstre            ia        = new IAMonstre(executeur);       // reçoit l'interface
ResolveurAction      resolver  = new ResolveurAction(executeur, input);
IMoteurCombat        moteur    = new MoteurCombat(executeur, ia, resolver);
```

Conséquence directe : pour **tester `MoteurCombat`** sans ouvrir une vraie console, il suffit d'injecter un `FakeInputHandler` — zéro modification du code existant.

---


---

## 🛠️ Étendre le projet

### Ajouter la classe Paladin

```csharp
// Classes/DefinitionPaladin.cs
public sealed class DefinitionPaladin : IDefinitionClasse
{
    public TypeClasse Classe      => TypeClasse.Paladin;
    public string     Icone       => "✙";
    public string     Description => "Guerrier sacré, hybride tank/heal";

    public Heros Creer(string nom)
    {
        var h = new Heros(nom, Icone, Classe,
            hp: 115, mp: 60, force: 13, intel: 12, agi: 8, def: 14, resM: 12);
        h.ApprendreCompetence(new("Frappe Divine", "✙", TypeCompetence.Magique, TypeCible.UnEnnemi, 35, coutMP: 12));
        // ...
        return h;
    }
}
```

```csharp
// Program.cs — ajouter une ligne
RegistreClasses registre = new RegistreClasses([
    new DefinitionGuerrier(),
    new DefinitionMage(),
    new DefinitionVoleur(),
    new DefinitionClerc(),
    new DefinitionPaladin(), // ← ici
]);
```

Aucun autre fichier modifié.

---

## 📚 Stack technique

- **Langage** : C# 12
- **Runtime** : .NET 8
- **UI** : Console avec codes ANSI (couleurs, barres HP/MP)
- **Patterns** : Strategy, Repository, Composition Root, Dependency Injection manuelle
- **Paradigme** : POO avec interfaces, pas de framework DI externe

---

## 👤 Auteur

**Auguy Mabika** —  Geneva Institute of Technology
