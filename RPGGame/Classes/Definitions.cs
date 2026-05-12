using RPGGame.Interfaces;
using RPGGame.Models;

namespace RPGGame.Classes;

// ════════════════════════════════════════════════════════════════════════════
// OCP en action : pour ajouter une 5ème classe (ex: Paladin),
// on CRÉE un nouveau fichier DefinitionPaladin.cs et on l'enregistre
// dans Program.cs. Aucun code existant n'est touché.
// ════════════════════════════════════════════════════════════════════════════

public sealed class DefinitionGuerrier : IDefinitionClasse
{
    public TypeClasse Classe      => TypeClasse.Guerrier;
    public string     Icone       => "⚔";
    public string     Description => "Tank robuste, maître du combat physique";

    public Heros Creer(string nom)
    {
        var h = new Heros(nom, Icone, Classe,
            hp: 140, mp: 25, force: 16, intel: 5, agi: 9, def: 13, resM: 5);
        h.ApprendreCompetence(new("Attaque",       "⚔", TypeCompetence.Physique, TypeCible.UnEnnemi,    20));
        h.ApprendreCompetence(new("Frappe Lourde", "💥", TypeCompetence.Physique, TypeCible.UnEnnemi,    38));
        h.ApprendreCompetence(new("Cri de Guerre", "📣", TypeCompetence.Buff,     TypeCible.Soi,          0, coutMP: 8));
        h.ApprendreCompetence(new("Défense",       "🛡", TypeCompetence.Defendre, TypeCible.Soi,          0));
        return h;
    }
}

public sealed class DefinitionMage : IDefinitionClasse
{
    public TypeClasse Classe      => TypeClasse.Mage;
    public string     Icone       => "✦";
    public string     Description => "Puissance magique dévastatrice, très fragile";

    public Heros Creer(string nom)
    {
        var h = new Heros(nom, Icone, Classe,
            hp: 72, mp: 110, force: 5, intel: 22, agi: 8, def: 4, resM: 15);
        h.ApprendreCompetence(new("Attaque",      "⚔",  TypeCompetence.Physique, TypeCible.UnEnnemi,     9));
        h.ApprendreCompetence(new("Boule de Feu", "🔥", TypeCompetence.Magique,  TypeCible.UnEnnemi,    45, coutMP: 15));
        h.ApprendreCompetence(new("Blizzard",     "❄",  TypeCompetence.Magique,  TypeCible.TousEnnemis, 30, coutMP: 24, effet: EffetStatut.Ralenti));
        h.ApprendreCompetence(new("Drain",        "🌑", TypeCompetence.Magique,  TypeCible.UnEnnemi,    24, coutMP: 12));
        return h;
    }
}

public sealed class DefinitionVoleur : IDefinitionClasse
{
    public TypeClasse Classe      => TypeClasse.Voleur;
    public string     Icone       => "†";
    public string     Description => "Agile et rapide, expert des coups critiques";

    public Heros Creer(string nom)
    {
        var h = new Heros(nom, Icone, Classe,
            hp: 92, mp: 45, force: 14, intel: 9, agi: 22, def: 7, resM: 6);
        h.ApprendreCompetence(new("Attaque",          "⚔",  TypeCompetence.Physique, TypeCible.UnEnnemi, 16));
        h.ApprendreCompetence(new("Coup Critique",    "💢", TypeCompetence.Physique, TypeCible.UnEnnemi, 42, critique: 0.45f));
        h.ApprendreCompetence(new("Lame Empoisonnée", "☠",  TypeCompetence.Physique, TypeCible.UnEnnemi, 14, coutMP: 8, effet: EffetStatut.Poison));
        h.ApprendreCompetence(new("Double Frappe",    "⚡", TypeCompetence.Physique, TypeCible.UnEnnemi, 18, coutMP: 10, coups: 2));
        return h;
    }
}

public sealed class DefinitionClerc : IDefinitionClasse
{
    public TypeClasse Classe      => TypeClasse.Clerc;
    public string     Icone       => "✶";
    public string     Description => "Guérisseur sacré, soutien indispensable";

    public Heros Creer(string nom)
    {
        var h = new Heros(nom, Icone, Classe,
            hp: 88, mp: 95, force: 9, intel: 18, agi: 7, def: 11, resM: 18);
        h.ApprendreCompetence(new("Attaque",        "⚔",  TypeCompetence.Physique, TypeCible.UnEnnemi,   12));
        h.ApprendreCompetence(new("Soin",           "💚", TypeCompetence.Soin,     TypeCible.UnAllie,    55, coutMP: 14));
        h.ApprendreCompetence(new("Soin de Groupe", "💖", TypeCompetence.Soin,     TypeCible.TousAllies, 32, coutMP: 30));
        h.ApprendreCompetence(new("Lumière Sacrée", "☀",  TypeCompetence.Magique,  TypeCible.UnEnnemi,   40, coutMP: 20));
        return h;
    }
}
